namespace SupportDesk.Application.Features.Alerts
{
    public interface IPendingRequestsAlertService
    {
        Task SendPendingRequestsAlertAsync(CancellationToken cancellationToken = default);
    }
}
