using Microsoft.Extensions.DependencyInjection;
using SupportDesk.Application.Contracts.Services;
using SupportDesk.Application.Features.Alerts;
using SupportDesk.Application.Features.Reports;
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

        services.AddScoped<IExpiringRequestsAlertService, ExpiringRequestsAlertService>();
        services.AddScoped<IPendingRequestsAlertService, PendingRequestsAlertService>();
        services.AddScoped<IDailyReportService, DailyReportService>();
        services.AddScoped<IWeeklyReportService, WeeklyReportService>();
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<ITwoFactorService, TwoFactorService>();

        return services;
    }
}
