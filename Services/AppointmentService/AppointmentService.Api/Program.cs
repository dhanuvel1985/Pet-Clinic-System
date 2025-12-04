using AppointmentService.Api.BackgroundServices;
using AppointmentService.Application.Commands;
using AppointmentService.Application.Interfaces;
using AppointmentService.Application.Queries;
using AppointmentService.Infrastructure;
using AppointmentService.Infrastructure.External;
using AppointmentService.Infrastructure.Notifications;
using AppointmentService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Shared.Resilience;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppointmentDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

//builder.Services.AddHttpClient<IPetServiceClient, PetServiceClient>();

builder.Services.AddScoped<IAppointmentRepository, AppointmentRepository>();

builder.Services.AddHttpContextAccessor();

//builder.Services.AddScoped<IUserServiceClient, UserServiceClient>();
var serviceUrl = builder.Configuration.GetSection("Services");
builder.Services.AddHttpClient<IPetServiceClient, PetServiceClient>(client =>
{
    client.BaseAddress = new Uri(serviceUrl["PetServiceUrl"]); // API Gateway
})
.AddPolicyHandler(PollyPolicies.GetRetryPolicy())
.AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy())
.ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
}); 

builder.Services.AddHttpClient<IUserServiceClient, UserServiceClient>(client =>
{
    client.BaseAddress = new Uri(serviceUrl["UserServiceUrl"]); // API Gateway
})
.AddPolicyHandler(PollyPolicies.GetRetryPolicy())
.AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy())
.ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});

//builder.Services.AddScoped<IMessageBusPublisher, ServiceBusMessagePublisher>();
builder.Services.AddSingleton<INotificationService, NotificationService>();

builder.Services.AddScoped<CreateAppointmentCommandHandler>();
builder.Services.AddScoped<GetAppointmentQueryHandler>();
builder.Services.AddScoped<AcceptAppointmentCommandHandler>();

//builder.Services.AddHostedService<AppointmentCreatedConsumer>();

builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .WriteTo.Console()
        .WriteTo.File("logs/log.txt", rollingInterval: RollingInterval.Day);
});

// 6. Auth
var config = builder.Configuration.GetSection("Jwt");
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = config["Issuer"],

            ValidateAudience = true,
            ValidAudience = config["Audience"],

            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(config["Secret"])),

            ValidateLifetime = true
        };
        //options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
        //{
        //    OnAuthenticationFailed = ctx => {
        //        Console.WriteLine("JWT ERROR: " + ctx.Exception?.Message);
        //        return Task.CompletedTask;
        //    }
        //};
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = ctx =>
            {
                Console.WriteLine("Gateway OnMessageReceived token: " + ctx.Token);
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = ctx =>
            {
                Console.WriteLine("Gateway JWT FAILED: " + ctx.Exception?.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = ctx =>
            {
                Console.WriteLine("Gateway token valid. Claims: " +
                    string.Join(", ", ctx.Principal.Claims.Select(c => c.Type + "=" + c.Value)));
                return Task.CompletedTask;
            }
        };
    });

// 7. OpenTelemetry
builder.Services.AddOpenTelemetry()
    .WithTracing(t =>
    {
        t.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("AppointmentService"))
         .AddAspNetCoreInstrumentation()
         .AddHttpClientInstrumentation()
         .AddOtlpExporter(o =>
         {
             o.Endpoint = new Uri(builder.Configuration["OpenTelemetry:Endpoint"]);
         });
    });

var app = builder.Build();

app.UseSerilogRequestLogging();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseSerilogRequestLogging();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
