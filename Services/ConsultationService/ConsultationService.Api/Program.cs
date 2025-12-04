using ConsultationService.Application.Commands;
using ConsultationService.Application.Interfaces;
using ConsultationService.Application.Mappings;
using ConsultationService.Application.Queries;
using ConsultationService.Infrastructure;
using ConsultationService.Infrastructure.External;
using ConsultationService.Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Resilience;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ConsultationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});


builder.Services.AddScoped<IConsultationRepository, ConsultationRepository>();

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

builder.Services.AddHttpClient<IAppointmentServiceClient, AppointmentServiceClient>(client =>
{
    client.BaseAddress = new Uri(serviceUrl["AppointmentServiceUrl"]); // API Gateway
})
.AddPolicyHandler(PollyPolicies.GetRetryPolicy())
.AddPolicyHandler(PollyPolicies.GetCircuitBreakerPolicy())
.ConfigurePrimaryHttpMessageHandler(() =>
        new HttpClientHandler
        {
            ServerCertificateCustomValidationCallback =
                HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        });

builder.Services.AddScoped<CreateConsultationCommandHandler>();
builder.Services.AddScoped<GetAllConsultationQueryHandler>();
builder.Services.AddScoped<GetConsultationByIdQueryHandler>();

// Add services to the container.
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

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// MediatR
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(CreateConsultationCommand).Assembly));

// AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
