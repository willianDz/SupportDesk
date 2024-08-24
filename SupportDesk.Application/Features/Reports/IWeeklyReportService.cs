namespace SupportDesk.Application.Features.Reports;

public interface IWeeklyReportService
{
    Task SendWeeklyReportAsync(CancellationToken cancellationToken = default);
}