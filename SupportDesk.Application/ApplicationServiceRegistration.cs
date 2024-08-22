using Microsoft.Extensions.DependencyInjection;
using SupportDesk.Application.Contracts.Services;
using SupportDesk.Application.Services;
using System.Reflection;

namespace SupportDesk.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));
        services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

        services.AddTransient<IRequestValidationService, RequestValidationService>();

        return services;
    }
}
