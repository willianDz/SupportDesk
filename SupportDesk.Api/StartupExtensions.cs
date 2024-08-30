using SupportDesk.Application;
using SupportDesk.Persistence;
using SupportDesk.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SupportDesk.Api.Auth;
using Asp.Versioning;
using SupportDesk.Api.Endpoints;
using Microsoft.Extensions.FileProviders;
using System.Text.Json.Serialization;
using Quartz;
using SupportDesk.Application.Features.Jobs;
using SupportDesk.Api.Mapping;


namespace SupportDesk.Api;

public static class StartupExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddApplicationServices();
        builder.Services.AddPersistenceServices(builder.Configuration);
        builder.Services.AddInfrastructureServices(builder.Configuration);

        builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
        builder.Services.AddAntiforgery();

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.CreateApiVersionSet();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("UAT"))
        {
            app.UseSwagger();
            app.UseSwaggerUI(x =>
            {
                foreach (var description in app.DescribeApiVersions())
                {
                    x.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
                        description.GroupName);
                }
            });
        }

        // Configurar la ruta para servir archivos estáticos
        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(CreateAndGetFileStorageDirectory()),
            RequestPath = "/files"
        });

        app.MapHealthChecks("hc");
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseOutputCache();
        app.UseMiddleware<ValidationMappingMiddleware>();
        app.UseAntiforgery();
        app.MapApiEndpoints();

        return app;
    }

    public static IServiceCollection ConfigureAuthentication(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        if (string.IsNullOrEmpty(configuration["Jwt:Key"]) || string.IsNullOrEmpty(configuration["Jwt:Issuer"]) || string.IsNullOrEmpty(configuration["Jwt:Audience"]))
        {
            throw new ArgumentException("JWT configuration is invalid");
        }

        services.AddAuthentication(x =>
        {
            x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(x =>
        {
            x.TokenValidationParameters = new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
                ValidateIssuerSigningKey = true,
                ValidateLifetime = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                ValidateIssuer = true,
                ValidateAudience = true
            };
        });

        return services;

    }

    public static IServiceCollection ConfigureAuthorization(this IServiceCollection services)
    {
        services.AddAuthorization(x =>
        {
            x.AddPolicy(AuthConstants.AdminUserPolicyName,
                p => p.RequireClaim(AuthConstants.AdminUserClaimName, "true"));

            x.AddPolicy(AuthConstants.SupervisorUserPolicyName,
                p => p.RequireAssertion(c =>
                    c.User.HasClaim(m => m is { Type: AuthConstants.AdminUserClaimName, Value: "true" }) ||
                    c.User.HasClaim(m => m is { Type: AuthConstants.SupervisorUserClaimName, Value: "true" })));

            x.AddPolicy(AuthConstants.TrustedMemberPolicyName,
                p => p.RequireAssertion(c =>
                    c.User.HasClaim(m => m is { Type: AuthConstants.AdminUserClaimName, Value: "true" }) ||
                    c.User.HasClaim(m => m is { Type: AuthConstants.SupervisorUserClaimName, Value: "true" }) ||
                    c.User.HasClaim(m => m is { Type: AuthConstants.TrustedMemberClaimName, Value: "true" })));
        });

        return services;
    }

    public static IServiceCollection ConfigureApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(x =>
        {
            x.DefaultApiVersion = new ApiVersion(1.0);
            x.AssumeDefaultVersionWhenUnspecified = true;
            x.ReportApiVersions = true;
            x.ApiVersionReader = new MediaTypeApiVersionReader("api-version");
        }).AddApiExplorer();

        return services;
    }

    public static IServiceCollection ConfigureScheduledTasks(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.UseMicrosoftDependencyInjectionJobFactory();

            // Configuración del Job de Solicitudes Pendientes
            q.AddJob<PendingRequestsAlertJob>(opts => opts.WithIdentity("PendingRequestsAlertJob"));
            q.AddTrigger(opts => opts
                .ForJob("PendingRequestsAlertJob")
                .WithIdentity("PendingRequestsAlertTrigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(12)
                    .RepeatForever()));

            // Configuración del Job de Solicitudes Próximas a Expirar
            q.AddJob<ExpiringRequestsAlertJob>(opts => opts.WithIdentity("ExpiringRequestsAlertJob"));
            q.AddTrigger(opts => opts
                .ForJob("ExpiringRequestsAlertJob")
                .WithIdentity("ExpiringRequestsAlertTrigger")
                .StartNow()
                .WithSimpleSchedule(x => x
                    .WithIntervalInHours(24)
                    .RepeatForever()));

            // Configuracion del job de informe diario
            q.AddJob<DailyReportJob>(opts => opts.WithIdentity("DailyReportJob"));
            q.AddTrigger(opts => opts
                .ForJob("DailyReportJob")
                .WithIdentity("DailyReportTrigger")
                .StartNow()
                .WithCronSchedule("0 0 0 * * ?")); // Ejecutar a medianoche todos los días

            // Configuración del job de infore semanal para los admins
            q.AddJob<WeeklyReportJob>(opts => opts.WithIdentity("WeeklyReportJob"));
            q.AddTrigger(opts => opts
                .ForJob("WeeklyReportJob")
                .WithIdentity("WeeklyReportTrigger")
                .StartNow()
                .WithCronSchedule("0 0 12 ? * MON *")); // Ejecución cada lunes a las 12 PM
        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }

    public static IServiceCollection ConfigureOutputCache(this IServiceCollection services)
    {
        services.AddOutputCache(x =>
        {
            x.AddBasePolicy(c => c.Cache());

            x.AddPolicy("ExampleCachePolicy", c =>
                c.Cache()
                .Expire(TimeSpan.FromMinutes(5))
                .SetVaryByQuery(new[] { "description", "sortBy", "page", "pageSize" })
                .Tag("example"));
        });

        return services;
    }

    public static IServiceCollection ConfigureHealthChecks(this IServiceCollection services)
    {
        services.AddHealthChecks();

        return services;
    }

    private static string CreateAndGetFileStorageDirectory()
    {
        var directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "FileStorage");
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        return directoryPath;
    }
}
