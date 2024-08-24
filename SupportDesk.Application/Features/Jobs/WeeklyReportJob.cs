using Quartz;
using SupportDesk.Application.Features.Reports;

namespace SupportDesk.Application.Features.Jobs
{
    public class WeeklyReportJob : IJob
    {
        private readonly IWeeklyReportService _weeklyReportService;

        public WeeklyReportJob(IWeeklyReportService weeklyReportService)
        {
            _weeklyReportService = weeklyReportService;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await _weeklyReportService.SendWeeklyReportAsync(context.CancellationToken);
        }
    }
}
