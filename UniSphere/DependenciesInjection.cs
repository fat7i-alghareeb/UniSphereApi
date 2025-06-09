using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using UniSphere.Api.Database;

namespace UniSphere.Api;

public static class DependenciesInjection
{
    public static WebApplicationBuilder AddControllers(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers();
        builder.Services.AddOpenApi();

        return builder;
    }

    public static WebApplicationBuilder AddErrorHanding(this WebApplicationBuilder builder)
    {

        return builder;
    }

    public static WebApplicationBuilder AddDatabase(this WebApplicationBuilder builder)
    {
        builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(
                builder.Configuration.GetConnectionString("Database"),
                sqlserverOptions => sqlserverOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemes.Application)
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

        return builder;
    }
}



