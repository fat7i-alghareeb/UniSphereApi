using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using UniSphere.Api.Database;
using UniSphere.Api.Database.Seeding;
using UniSphere.Api.Middleware;

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
        var dataSourceBuilder = new NpgsqlDataSourceBuilder(
            builder.Configuration.GetConnectionString("Database")!
        );

        // ✅ Enable dynamic JSON using System.Text.Json
        dataSourceBuilder.EnableDynamicJson();

        // Optionally: use Newtonsoft.Json instead
        // dataSourceBuilder.UseJsonNet();

        NpgsqlDataSource? dataSource = dataSourceBuilder.Build();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(dataSource, npgsqlOptions =>
                npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemes.Application)
            ).UseSnakeCaseNamingConvention()
        );

        // Register seeders
        builder.Services.AddScoped<UniversitySeedData>();
        builder.Services.AddScoped<FacultySeedData>();
        builder.Services.AddScoped<MajorSeedData>();
        builder.Services.AddScoped<EnrollmentStatusSeedData>();
        builder.Services.AddScoped<ProfessorSeedData>();
        builder.Services.AddScoped<SubjectSeedData>();
        builder.Services.AddScoped<StudentCredentialSeedData>();
        builder.Services.AddScoped<SubjectProfessorLinkSeedData>();
        builder.Services.AddScoped<SubjectStudentLinkSeedData>();
        builder.Services.AddScoped<LabSeedData>();
        builder.Services.AddScoped<InstructorSeedData>();
        builder.Services.AddScoped<InstructorLabLinkSeedData>();
        builder.Services.AddScoped<ScheduleSeedData>();
        builder.Services.AddScoped<LectureSeedData>();
        builder.Services.AddScoped<DatabaseSeeder>();

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



