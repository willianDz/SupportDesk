using Microsoft.Extensions.Logging;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Models.Notifications;
using System.Text;

namespace SupportDesk.Application.Features.Reports;

public class DailyReportService : IDailyReportService
{
    private readonly IRequestRepository _requestRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<DailyReportService> _logger;

    public DailyReportService(
        IRequestRepository requestRepository,
        IUserRepository userRepository,
        INotificationService notificationService,
        ILogger<DailyReportService> logger)
    {
        _requestRepository = requestRepository;
        _userRepository = userRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task SendDailyReportAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Obtener las estadísticas del día
            var today = DateTime.UtcNow.Date;
            var createdRequestsCount = await _requestRepository.GetRequestsCountByDateAsync(today, cancellationToken);
            var processedRequestsCount = await _requestRepository.GetProcessedRequestsCountByDateAsync(today, cancellationToken);
            var pendingRequestsCount = await _requestRepository.GetPendingRequestsCountAsync(cancellationToken);
            var averageResponseTime = await _requestRepository.GetAverageResponseTimeAsync(cancellationToken);

            // Obtener los administradores
            var adminUsers = await _userRepository.GetAdminUsersAsync(cancellationToken);

            if (adminUsers.Count == 0)
            {
                return;
            }

            // Crear el mensaje de notificación
            var notificationMessage = new NotificationMessage
            {
                RecipientUserIds = adminUsers.Select(u => u.Id).ToList(),
                Subject = "Informe Diario de Solicitudes",
                Body = BuildReportBody(createdRequestsCount, processedRequestsCount, pendingRequestsCount, averageResponseTime)
            };

            // Enviar el informe
            await _notificationService.SendNotificationAsync(notificationMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ReportsMessages.FailedToSendDailyReport);
        }
    }

    private string BuildReportBody(
        int createdRequestsCount,
        int processedRequestsCount,
        int pendingRequestsCount,
        TimeSpan averageResponseTime)
    {
        var body = new StringBuilder();
        body.AppendLine("Informe Diario de Solicitudes:");
        body.AppendLine($"- Número total de solicitudes creadas hoy: {createdRequestsCount}");
        body.AppendLine($"- Número total de solicitudes procesadas (aprobadas/rechazadas): {processedRequestsCount}");
        body.AppendLine($"- Número de solicitudes pendientes de revisión: {pendingRequestsCount}");
        body.AppendLine($"- Tiempo promedio de respuesta: {averageResponseTime.TotalHours:F2} horas");

        return body.ToString();
    }
}
