using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportDesk.Application.Contracts.Infraestructure.FileStorage;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Contracts.Infraestructure.Security;
using SupportDesk.Infrastructure.FileStorage;
using SupportDesk.Infrastructure.Notifications;
using SupportDesk.Infrastructure.Security;

namespace SupportDesk.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        if (string.IsNullOrEmpty(configuration["Jwt:Key"]) || string.IsNullOrEmpty(configuration["Jwt:Issuer"]) || string.IsNullOrEmpty(configuration["Jwt:Audience"]))
        {
            throw new ArgumentException("JWT configuration is invalid");
        }

        services.AddTransient<ITokenGenerator>(r => new TokenGenerator(
            tokenSecret: configuration["Jwt:Key"]!,
            tokenLifetimeInMinutes: double.Parse(configuration["Jwt:tokenLifetimeInMinutes"]!),
            issuer: configuration["Jwt:Issuer"]!,
            audience: configuration["Jwt:Audience"]!
        ));

        // Registro de IHttpContextAccessor
        services.AddHttpContextAccessor();

        // Resolver storagePath desde IConfiguration
        var storagePath = configuration["FileStorage:Path"] ?? "FileStorage";

        // Construir baseUrl dinámicamente usando IHttpContextAccessor
        services.AddScoped<IFileStorageService>(provider =>
        {
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            var baseUrl = $"{httpContextAccessor.HttpContext?.Request.Scheme}://{httpContextAccessor.HttpContext?.Request.Host.Value}/files/";

            return new LocalFileStorageService(storagePath, baseUrl);
        });

        services.AddTransient<INotificationService, SendGridNotificationService>();

        return services;
    }
}
