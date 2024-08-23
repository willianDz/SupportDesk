using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportDesk.Application.Contracts.Infraestructure.FileStorage;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Contracts.Infraestructure.Security;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Infrastructure.FileStorage;
using SupportDesk.Infrastructure.Notifications;
using SupportDesk.Infrastructure.Security;

namespace SupportDesk.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        RegisterTokenGeneratorService(services, configuration);
        RegisterLocalFileStorageService(services, configuration);
        RegisterSmtpNotificationService(services, configuration);

        return services;
    }

    private static void RegisterTokenGeneratorService(IServiceCollection services, IConfiguration configuration)
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
    }

    private static void RegisterLocalFileStorageService(IServiceCollection services, IConfiguration configuration)
    {
        var storagePath = configuration["FileStorage:Path"] ?? "FileStorage";

        services.AddScoped<IFileStorageService>(provider =>
        {
            var httpContextAccessor = provider.GetRequiredService<IHttpContextAccessor>();
            var baseUrl = $"{httpContextAccessor.HttpContext?.Request.Scheme}://{httpContextAccessor.HttpContext?.Request.Host.Value}/files/";

            return new LocalFileStorageService(storagePath, baseUrl);
        });
    }

    private static void RegisterSmtpNotificationService(IServiceCollection services, IConfiguration configuration)
    {
        bool isSmtpConfigurationinvalid = string.IsNullOrEmpty(configuration["Smtp:Server"])
            || string.IsNullOrEmpty(configuration["Smtp:Port"])
            || string.IsNullOrEmpty(configuration["Smtp:Username"])
            || string.IsNullOrEmpty(configuration["Smtp:Password"]);

        if (isSmtpConfigurationinvalid)
        {
            throw new ArgumentException("SMTP configuration is invalid");
        }

        var smtpServer = configuration["Smtp:Server"]!;
        var smtpPort = int.Parse(configuration["Smtp:Port"]!);
        var smtpUsername = configuration["Smtp:Username"]!;
        var smtpPassword = configuration["Smtp:Password"]!;

        //services.AddTransient<INotificationService>(provider => new SmtpNotificationService(
        //    smtpServer, 
        //    smtpPort, 
        //    smtpUsername, 
        //    smtpPassword, 
        //    provider.GetRequiredService<IUserRepository>()));
    }
}
