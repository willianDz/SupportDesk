using Microsoft.Extensions.DependencyInjection;

namespace SupportDesk.Application;

public static class ApplicationServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        //services.AddMediatR(AppDomain.CurrentDomain.GetAssemblies());

        return services;
    }
}
