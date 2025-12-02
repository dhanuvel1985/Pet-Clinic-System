using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using PetService.Api.Services;
using PetService.Application.Commands;
using PetService.Application.Interfaces;
using PetService.Application.Validators;
using PetService.Infrastructure;
using PetService.Infrastructure.Repositories;
using Serilog;
using System.Text;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        // 1. Serilog
        builder.Host.UseSerilog((ctx, lc) =>
            lc.ReadFrom.Configuration(ctx.Configuration)
              .Enrich.FromLogContext());

        // 2. DbContext
        var conn = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<PetDbContext>(opts => opts.UseSqlServer(conn,
            sql => sql.EnableRetryOnFailure(5)));

        // 3. DI
        builder.Services.AddScoped<IPetRepository, PetRepository>();
        builder.Services.AddScoped<ICurrentUserService, CurrentUserService>();
        builder.Services.AddHttpContextAccessor();

        //builder.Services.AddAutoMapper(typeof(Program));
        builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        // 4. MediatR
        builder.Services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(CreatePetCommand).Assembly);
        });

        // 5. FluentValidation
        builder.Services.AddValidatorsFromAssembly(typeof(CreatePetCommand).Assembly);
        builder.Services.AddValidatorsFromAssemblyContaining<CreatePetValidator>();

        // 6. Auth
        var config = builder.Configuration.GetSection("Jwt");
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = "Bearer";
            options.DefaultChallengeScheme = "Bearer";
        })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = config.GetValue<bool>("RequireHttpsMetadata");
                options.SaveToken = true;
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
                options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnAuthenticationFailed = ctx => {
                        Console.WriteLine("JWT ERROR: " + ctx.Exception?.Message);
                        return Task.CompletedTask;
                    }
                };
            });

        // 7. OpenTelemetry
        builder.Services.AddOpenTelemetry()
            .WithTracing(t =>
            {
                t.SetResourceBuilder(ResourceBuilder.CreateDefault().AddService("PetService"))
                 .AddAspNetCoreInstrumentation()
                 .AddHttpClientInstrumentation()
                 .AddOtlpExporter(o =>
                 {
                     o.Endpoint = new Uri(builder.Configuration["OpenTelemetry:Endpoint"]);
                 });
            });

        // 8. Controllers
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();


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

        app.Use(async (ctx, next) =>
        {
            Console.WriteLine("AUTH HEADER => " + ctx.Request.Headers["Authorization"]);
            await next();
        });

        app.Run();
    }
}