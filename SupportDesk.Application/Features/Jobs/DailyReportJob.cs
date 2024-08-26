using Quartz;
using SupportDesk.Application.Features.Reports;

namespace SupportDesk.Application.Features.Jobs;

public class DailyReportJob : IJob
{
    private readonly IDailyReportService _dailyReportService;

    public DailyReportJob(IDailyReportService dailyReportService)
    {
        _dailyReportService = dailyReportService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        await _dailyReportService.SendDailyReportAsync(context.CancellationToken);
    }
}
