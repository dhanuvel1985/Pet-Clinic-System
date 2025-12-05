using AuthService.Api.Middlewares;
using AuthService.Application.Commands;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Infrastructure;
using AuthService.Infrastructure.Repositories;
using CorrelationId;
using CorrelationId.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

// --- Configuration & Serilog
builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                     .AddEnvironmentVariables();

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .Enrich.WithCorrelationId()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// --- CorrelationId
builder.Services.AddDefaultCorrelationId(options =>
{
    options.IncludeInResponse = true;
    options.RequestHeader = "X-Correlation-ID";
    options.ResponseHeader = "X-Correlation-ID";
    options.UpdateTraceIdentifier = true;
});

// --- DbContext
var conn = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AuthDbContext>(opts => opts.UseSqlServer(conn,
    sql => sql.EnableRetryOnFailure(5)));

// --- Repositories & services
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenRepository, TokenRepository>();

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(RegisterCommand).Assembly);
});

// Password hasher (from Microsoft.AspNetCore.Identity)
builder.Services.AddSingleton<Microsoft.AspNetCore.Identity.IPasswordHasher<User>,
    Microsoft.AspNetCore.Identity.PasswordHasher<User>>();

// --- Authentication (token validation) - services & gateway will reuse these settings
var jwtSection = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = jwtSection.GetValue<bool>("RequireHttpsMetadata");
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = jwtSection["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSection["Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Secret"])),
        ValidateLifetime = true,
        ClockSkew = TimeSpan.FromSeconds(jwtSection.GetValue<int>("ClockSkewSeconds", 60))
    };
});

// Health checks & controllers
builder.Services.AddHealthChecks();
builder.Services.AddControllers();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// Add Swagger services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Pet Hospital API",
        Version = "v1"
    });

    // Enable JWT Authorization button in Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Id = "Bearer",
                    Type = ReferenceType.SecurityScheme
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Seed Admin user
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    db.Database.Migrate();
    //if (!db.Roles.Any())
    //{
    //    db.Roles.Add(new Role
    //    {
    //        Name = "Admin"
    //    });
    //    db.SaveChanges();
    //}

    if (!db.Users.Any())
    {
        db.Users.Add(new User
        {
            Email = "admin@email.com",
            PasswordHash = "admin",
            RoleId = 1
        });
        db.SaveChanges();
    }
}

// Middlewares
app.UseSerilogRequestLogging();
app.UseCorrelationId();

// Configure the HTTP request pipeline.

// Middleware Pipeline -------------------------------
// Always publish Swagger JSON, even in prod
app.UseSwagger(c =>
{
    // Optional: Serve Swagger JSON on custom route
    // c.RouteTemplate = "swagger/{documentName}/swagger.json";
});

var enableSwagger = builder.Configuration.GetValue<bool>("Swagger:Enable");
// Configure HTTP request pipeline
if (enableSwagger)
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Global exception middleware (simple)
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
