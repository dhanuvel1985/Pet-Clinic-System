// Program.cs - ApiGateway (Full production-ready)
// .NET 8 minimal hosting model
// Requires NuGet packages:
// - Ocelot
// - Serilog.AspNetCore
// - Serilog.Enrichers.CorrelationId
// - CorrelationId
// - CorrelationId.Abstractions
// - AspNetCoreRateLimit
// - OpenTelemetry.Extensions.Hosting
// - OpenTelemetry.Exporter.OpenTelemetryProtocol
// - OpenTelemetry.Instrumentation.AspNetCore
// - OpenTelemetry.Instrumentation.Http
// - Microsoft.AspNetCore.Authentication.JwtBearer

using AspNetCoreRateLimit;
using CorrelationId;
using CorrelationId.Abstractions;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;
using System.Net;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------------
// Configuration sources
// -----------------------------------------------------------------------------
builder.Configuration
    .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true) // required
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

// -----------------------------------------------------------------------------
// Serilog (structured JSON logs, enrichment including CorrelationId)
// -----------------------------------------------------------------------------
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)        // respects Serilog section in appsettings
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()                           // requires Serilog.Enrichers.CorrelationId
    .WriteTo.Console()                                    // can add Seq / file sinks here
    .CreateLogger();

builder.Host.UseSerilog();

// ADD THIS — You forgot it
builder.Services.AddControllers();

// -----------------------------------------------------------------------------
// Correlation ID middleware - propagate X-Correlation-ID and generate if missing
// -----------------------------------------------------------------------------
builder.Services.AddDefaultCorrelationId(options =>
{
    options.IncludeInResponse = true;
    options.RequestHeader = "X-Correlation-ID";
    options.ResponseHeader = "X-Correlation-ID";
    options.UpdateTraceIdentifier = true; // sync HttpContext.TraceIdentifier with correlation id
});

// -----------------------------------------------------------------------------
// Rate limiting - IpRateLimit (simple in-memory config); configure in appsettings
// -----------------------------------------------------------------------------
builder.Services.AddOptions();
builder.Services.AddMemoryCache();
builder.Services.Configure<IpRateLimitOptions>(builder.Configuration.GetSection("IpRateLimiting"));
builder.Services.AddInMemoryRateLimiting();
builder.Services.AddSingleton<IRateLimitConfiguration, RateLimitConfiguration>();

// -----------------------------------------------------------------------------
// OpenTelemetry Tracing (AspNetCore + HttpClient + OTLP exporter)
// -----------------------------------------------------------------------------
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("PetHospital.ApiGateway"))
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation();
        tracing.AddHttpClientInstrumentation();
        tracing.AddSource("PetHospital.ApiGateway"); // optional manual ActivitySource
        // OTLP exporter endpoint configured in appsettings: "OpenTelemetry:Endpoint"
        var endpoint = builder.Configuration["OpenTelemetry:Endpoint"];
        if (!string.IsNullOrWhiteSpace(endpoint))
        {
            tracing.AddOtlpExporter(opt => opt.Endpoint = new Uri(endpoint));
        }
    });

// -----------------------------------------------------------------------------
// Authentication (JWT) - Gateway validates tokens centrally
// -----------------------------------------------------------------------------
//var jwtAuthority = builder.Configuration["Jwt:Authority"];
//var jwtAudience = builder.Configuration["Jwt:Audience"];

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        // Minimal validation here; tune TokenValidationParameters as needed for prod
//        options.Authority = jwtAuthority;
//        options.Audience = jwtAudience;
//        options.RequireHttpsMetadata = builder.Configuration.GetValue<bool?>("Jwt:RequireHttpsMetadata") ?? true;
//        options.TokenValidationParameters = new TokenValidationParameters
//        {
//            ValidateIssuer = true,
//            ValidateAudience = true,
//            ValidateLifetime = true
//            // You may configure additional validation (IssuerSigningKey, ClockSkew, etc.)
//        };
//    });

//builder.Services.AddAuthentication()
//    .AddJwtBearer("Bearer", options =>
//    {
//        options.Authority = "https://localhost:7061";
//        options.Audience = "pet_api";
//        options.RequireHttpsMetadata = false;
//    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.RequireHttpsMetadata = false;
    options.Authority = "https://localhost:7061";
    options.Audience = "pet_api";

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = "https://localhost:7061",

        ValidateAudience = true,
        ValidAudience = "pet_api",

        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes("SuperSecretJwtKey123!trainingdemo@12345$SuperSecretJwtKey123!trainingdemo@12345$")),

        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromMinutes(2)
    };
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = ctx =>
        {
            Console.WriteLine("JWT FAILED: " + ctx.Exception.Message);
            return Task.CompletedTask;
        }
    };
});


// -----------------------------------------------------------------------------
// Health checks (basic); services behind gateway must expose readiness endpoints too
// -----------------------------------------------------------------------------
builder.Services.AddHealthChecks();

// -----------------------------------------------------------------------------
// Ocelot registration (reads ocelot.json)
// -----------------------------------------------------------------------------
builder.Services.AddOcelot(builder.Configuration);
builder.Services.AddAuthorization();
// -----------------------------------------------------------------------------
// Build app
// -----------------------------------------------------------------------------
var app = builder.Build();

// -----------------------------------------------------------------------------
// Global middleware pipeline
// -----------------------------------------------------------------------------

// Serilog request logging (structured)
app.UseSerilogRequestLogging(opts =>
{
    opts.MessageTemplate = "Handled {RequestPath}";
});

// Correlation ID middleware - must be before request logging to ensure logs contain id
app.UseCorrelationId();

// Use IP rate limiting early
app.UseIpRateLimiting();

// Global Exception handling for the Gateway (returns ProblemDetails-like JSON)
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        // Try to get correlation id safely
        string correlationId = null!;
        try
        {
            correlationId = context.Features.Get<ICorrelationContextAccessor>()
                    ?.CorrelationContext?.CorrelationId
                    ?? context.TraceIdentifier;
        }
        catch
        {
            correlationId = context.TraceIdentifier;
        }

        Log.Error(ex,
            "API Gateway unhandled exception. CorrelationId: {CorrelationId}, Path: {Path}",
            correlationId,
            context.Request.Path.ToString());

        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        context.Response.ContentType = "application/problem+json";

        var problem = new
        {
            type = "https://httpstatuses.io/500",
            title = "API Gateway Error",
            status = 500,
            detail = "An unexpected error occurred while processing the request.",
            instance = context.Request.Path,
            correlationId
        };

        await context.Response.WriteAsJsonAsync(problem);
    }
});

// Enable authentication/authorization on the gateway
app.UseAuthentication();
app.UseAuthorization();

// Health endpoints on gateway (use for gateway liveness)
app.MapHealthChecks("/health/live");
app.MapHealthChecks("/health/ready");
// Enable controller routing BEFORE Ocelot
app.MapControllers();

app.Use(async (ctx, next) =>
{
    Console.WriteLine("AUTH HEADER => " + ctx.Request.Headers["Authorization"]);
    await next();
});

// IMPORTANT: Ocelot middleware must be called last in the pipeline (it forwards requests)
try
{
    // UseOcelot() is asynchronous
    await app.UseOcelot();

    // Start the host
    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}
