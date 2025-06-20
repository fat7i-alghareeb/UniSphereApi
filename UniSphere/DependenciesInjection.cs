using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Npgsql;
using OpenTelemetry;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using UniSphere.Api.Database;
using UniSphere.Api.Database.Seeding;
using UniSphere.Api.Entities;
using UniSphere.Api.Middleware;
using UniSphere.Api.Services;
using UniSphere.Api.Settings;
using UniSphere.Api.Filters;

namespace UniSphere.Api;

public static class DependenciesInjection

{
    public static WebApplicationBuilder AddControllers(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers(options => 
        {
            options.ReturnHttpNotAcceptable = true;
            options.FormatterMappings.SetMediaTypeMappingForFormat("json", "application/json");
            options.Filters.Add(new ProducesAttribute("application/json"));
            options.Filters.Add(new LangHeaderFilter());
        })
        .AddNewtonsoftJson();

        builder.Services.AddSwaggerGen(options =>
        {
            options.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());
            options.AddSecurityDefinition("Bearer",
                new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter your JWT token like: Bearer {token}"
                });
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    { Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                    },
                    Array.Empty<string>()
                }
            });
            options.OperationFilter<AddLangHeaderOperationFilter>();
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

        NpgsqlDataSource dataSource = dataSourceBuilder.Build();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(dataSource, npgsqlOptions =>
                npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemes.Application)
            ).UseSnakeCaseNamingConvention()
        );
        builder.Services.AddDbContext<ApplicationIdentityDbContext>(options =>
            options.UseNpgsql(dataSource, npgsqlOptions =>
                npgsqlOptions.MigrationsHistoryTable(HistoryRepository.DefaultTableName, Schemes.Identity)
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
        builder.Services.AddScoped<FacultyAnnouncementSeedData>();
        builder.Services.AddScoped<MajorAnnouncementSeedData>();
        builder.Services.AddScoped<DatabaseSeeder>();
        builder.Services.AddScoped<AdminSeedData>();
        builder.Services.AddScoped<SuperAdminSeedData>();
        builder.Services.AddScoped<StudentStatisticsSeedData>();

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
        builder.Services.AddTransient<TokenProvider>();
        return builder;
    }

    public static WebApplicationBuilder AddAuthenticationServices(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationIdentityDbContext>();
        builder.Services.Configure<JwtAuthOptions>(builder.Configuration.GetSection("jwt"));
        JwtAuthOptions jwtAuthOptions = builder.Configuration.GetSection("Jwt").Get<JwtAuthOptions>()!;
        builder.Services
            .AddAuthentication(option =>
            {

                option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {

                    ValidIssuer = jwtAuthOptions.Issuer,
                    ValidAudience = jwtAuthOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        System.Text.Encoding.UTF8.GetBytes(jwtAuthOptions.Key)
                    )
                };
            });
        builder.Services.AddAuthorization();
        return builder;
    }

}



