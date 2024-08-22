using SupportDesk.Application;
using SupportDesk.Persistence;
using SupportDesk.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using SupportDesk.Api.Auth;
using Asp.Versioning;
using SupportDesk.Api.Endpoints;

namespace SupportDesk.Api;

public static class StartupExtensions
{
    public static WebApplication ConfigureServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddApplicationServices();
        builder.Services.AddPersistenceServices(builder.Configuration);
        builder.Services.AddInfrastructureServices(builder.Configuration);       

        return builder.Build();
    }

    public static WebApplication ConfigurePipeline(this WebApplication app)
    {
        app.CreateApiVersionSet();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
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

        app.MapHealthChecks("hc");
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseOutputCache();
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

            x.AddPolicy(AuthConstants.TrustedMemberPolicyName,
                p => p.RequireAssertion(c =>
                    c.User.HasClaim(m => m is { Type: AuthConstants.AdminUserClaimName, Value: "true" }) ||
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
}
