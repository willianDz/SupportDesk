namespace SupportDesk.Application.Features.Alerts;

public interface IExpiringRequestsAlertService
{
    Task SendExpiringRequestsAlertAsync(CancellationToken cancellationToken = default);
}
