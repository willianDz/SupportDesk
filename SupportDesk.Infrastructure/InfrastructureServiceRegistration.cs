using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SupportDesk.Application.Contracts.Infraestructure.Security;
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


        return services;
    }
}
