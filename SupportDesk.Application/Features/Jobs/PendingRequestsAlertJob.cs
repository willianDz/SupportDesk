using Quartz;
using SupportDesk.Application.Features.Alerts;

namespace SupportDesk.Application.Features.Jobs
{
    public class PendingRequestsAlertJob : IJob
    {
        private readonly IPendingRequestsAlertService _pendingRequestsAlertService;

        public PendingRequestsAlertJob(IPendingRequestsAlertService pendingRequestsAlertService)
        {
            _pendingRequestsAlertService = pendingRequestsAlertService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _pendingRequestsAlertService.SendPendingRequestsAlertAsync(context.CancellationToken);
        }
    }
}
