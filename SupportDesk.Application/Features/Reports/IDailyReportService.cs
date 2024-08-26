namespace SupportDesk.Application.Features.Reports;

public interface IDailyReportService
{
    Task SendDailyReportAsync(CancellationToken cancellationToken = default);
}
