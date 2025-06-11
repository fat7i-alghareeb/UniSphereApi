using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using UniSphere.Api.Database;
using UniSphere.Api.Middleware;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Npgsql.EntityFrameworkCore.PostgreSQL;

namespace UniSphere.Api;

public static class DependenciesInjection
{
    public static WebApplicationBuilder AddControllers(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen(options =>
        {
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
        });
        return builder;
    }

    public static WebApplicationBuilder AddErrorHandling(this WebApplicationBuilder builder)
    {
        builder.Services.AddProblemDetails(options =>
        {
            options.CustomizeProblemDetails = context =>
            {
                context.ProblemDetails.Extensions.Add("requestId", context.HttpContext.TraceIdentifier);
            };
        });
        builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        return builder;
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(
                builder.Configuration.GetConnectionString("Database"),
                npgsqlOptions => npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemes.Application)
            )
        );
        return builder;
    }
    /// <summary>
    /// Add OpenTelemetry for observability
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddObservability(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
            .WithTracing(tracing => tracing
                .AddHttpClientInstrumentation()
                .AddAspNetCoreInstrumentation())
            .WithMetrics(metrics => metrics
                .AddAspNetCoreInstrumentation()
                .AddHttpClientInstrumentation()
                .AddRuntimeInstrumentation())
            .UseOtlpExporter();

        builder.Logging.AddOpenTelemetry(options =>
            {
                options.IncludeScopes = true;
                options.IncludeFormattedMessage = true;

            }
        );
        return builder;
    }
    /// <summary>
    /// Add application services
    /// like validators ...etc 
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static WebApplicationBuilder AddApplicationServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddValidatorsFromAssemblyContaining<Program>();
        return builder;
    }
}



