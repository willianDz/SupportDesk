using Quartz;
using SupportDesk.Application.Features.Alerts;

namespace SupportDesk.Application.Features.Jobs
{
    public class ExpiringRequestsAlertJob : IJob
    {
        private readonly IExpiringRequestsAlertService _expiringRequestsAlertService;

        public ExpiringRequestsAlertJob(IExpiringRequestsAlertService expiringRequestsAlertService)
        {
            _expiringRequestsAlertService = expiringRequestsAlertService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _expiringRequestsAlertService.SendExpiringRequestsAlertAsync(context.CancellationToken);
        }
    }
}
