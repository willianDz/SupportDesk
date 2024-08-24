using Microsoft.Extensions.Logging;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Models.Notifications;
using SupportDesk.Domain.Enums;
using System.Text;

namespace SupportDesk.Application.Features.Reports
{
    public class WeeklyReportService : IWeeklyReportService
    {
        private readonly IRequestRepository _requestRepository;
        private readonly IUserRepository _userRepository;
        private readonly INotificationService _notificationService;
        private readonly ILogger<WeeklyReportService> _logger;

        public WeeklyReportService(
            IRequestRepository requestRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            ILogger<WeeklyReportService> logger)
        {
            _requestRepository = requestRepository;
            _userRepository = userRepository;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task SendWeeklyReportAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                var endDate = DateTime.UtcNow;
                var startDate = endDate.AddDays(-7);
                var previousStartDate = startDate.AddDays(-7);
                var previousEndDate = endDate.AddDays(-7);

                // Resumen de solicitudes por estado
                var createdCount = await _requestRepository.GetRequestCountByStatusAsync((int)RequestStatusesEnum.New, startDate, endDate, cancellationToken);
                var processedCount = await _requestRepository.GetRequestCountByStatusAsync((int)RequestStatusesEnum.Approved, startDate, endDate, cancellationToken);
                var rejectedCount = await _requestRepository.GetRequestCountByStatusAsync((int)RequestStatusesEnum.Rejected, startDate, endDate, cancellationToken);
                var underReviewCount = await _requestRepository.GetRequestCountByStatusAsync((int)RequestStatusesEnum.UnderReview, startDate, endDate, cancellationToken);

                // Comparación con la semana anterior
                var currentWeekCounts = await _requestRepository.GetWeeklyRequestCountsAsync(startDate, endDate, cancellationToken);
                var previousWeekCounts = await _requestRepository.GetWeeklyRequestCountsAsync(previousStartDate, previousEndDate, cancellationToken);

                // Tendencias por tipo de solicitud y zonas
                var trends = await _requestRepository.GetRequestTrendsByTypeAndZoneAsync(startDate, endDate, cancellationToken);

                // Tiempo promedio de resolución
                var averageResolutionTime = await _requestRepository.GetAverageResolutionTimeAsync(startDate, endDate, cancellationToken);

                // Construir cuerpo del informe
                var body = new StringBuilder();
                body.AppendLine("Informe Semanal de Solicitudes:");
                body.AppendLine($"- Solicitudes creadas: {createdCount}");
                body.AppendLine($"- Solicitudes procesadas: {processedCount}");
                body.AppendLine($"- Solicitudes rechazadas: {rejectedCount}");
                body.AppendLine($"- Solicitudes en revisión: {underReviewCount}");
                body.AppendLine();
                body.AppendLine("Comparación con la semana anterior:");

                if (currentWeekCounts is not null)
                {
                    foreach (var status in currentWeekCounts.Keys)
                    {
                        var previousCount = previousWeekCounts.ContainsKey(status) ? previousWeekCounts[status] : 0;
                        var difference = currentWeekCounts[status] - previousCount;
                        body.AppendLine($"- Estado {status}: {currentWeekCounts[status]} ({(difference >= 0 ? "+" : "")}{difference} en comparación con la semana anterior)");
                    }
                }
                
                body.AppendLine();
                body.AppendLine("Tendencias por tipo de solicitud y zonas:");

                if (trends is not null)
                {
                    foreach (var trend in trends)
                    {
                        body.AppendLine($"- Tipo de Solicitud {trend.RequestTypeId}, Zona {trend.ZoneId}: {trend.Count} solicitudes");
                    }
                }

                body.AppendLine();
                body.AppendLine($"Tiempo promedio de resolución: {averageResolutionTime.TotalHours:F2} horas");

                // Obtener administradores
                var admins = await _userRepository.GetAdminUsersAsync(cancellationToken);

                // Crear notificación
                var notificationMessage = new NotificationMessage
                {
                    RecipientUserIds = admins.Select(a => a.Id).ToList(),
                    Subject = "Informe Semanal de Solicitudes",
                    Body = body.ToString()
                };

                // Enviar notificación
                await _notificationService.SendNotificationAsync(notificationMessage, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ReportsMessages.FailedToSendWeeklyReport);
            }
        }
    }
}
