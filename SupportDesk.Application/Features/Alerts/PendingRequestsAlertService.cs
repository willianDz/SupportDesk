using Microsoft.Extensions.Logging;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Models.Notifications;
using SupportDesk.Domain.Entities;
using System.Text;

namespace SupportDesk.Application.Features.Alerts;

public class PendingRequestsAlertService : IPendingRequestsAlertService
{
    private readonly IRequestRepository _requestRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<PendingRequestsAlertService> _logger;

    public PendingRequestsAlertService(
        IRequestRepository requestRepository,
        IUserRepository userRepository,
        INotificationService notificationService,
        ILogger<PendingRequestsAlertService> logger)
    {
        _requestRepository = requestRepository;
        _userRepository = userRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task SendPendingRequestsAlertAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Definir el umbral de tiempo para considerar las solicitudes como "pendientes"
            var pendingThreshold = TimeSpan.FromHours(12); // Más de 12 horas

            // Obtener solicitudes que han estado pendientes por más de 12 horas y que aún no están bajo revisión
            var pendingRequests = await _requestRepository.GetPendingRequestsAsync(pendingThreshold, cancellationToken);

            if (pendingRequests.Count == 0)
            {
                return;
            }

            // Agrupar solicitudes por zona y tipo de solicitud
            var groupedRequests = pendingRequests
                .GroupBy(r => new { r.ZoneId, r.RequestTypeId })
                .ToList();

            foreach (var group in groupedRequests)
            {
                // Obtener los supervisores responsables de la zona y tipo de solicitud
                var supervisorIds = await _userRepository.GetSupervisorsByZoneAndRequestTypeAsync(
                    group.Key.ZoneId,
                    group.Key.RequestTypeId,
                    cancellationToken);

                if (supervisorIds is null || supervisorIds.Count == 0)
                {
                    continue;
                }

                // Crear el mensaje de notificación
                var notificationMessage = new NotificationMessage
                {
                    RecipientUserIds = supervisorIds.Select(u => u.Id).ToList(),
                    Subject = "Alerta: Solicitudes pendientes de revisión",
                    Body = BuildAlertBody(group.ToList())
                };

                // Enviar notificación
                await _notificationService.SendNotificationAsync(notificationMessage, cancellationToken);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, AlertsMessages.FailedToSendPendingRequestsAlert);
        }
    }

    private string BuildAlertBody(IList<Request> pendingRequests)
    {
        // Crear el cuerpo del mensaje con la lista de solicitudes pendientes
        var body = new StringBuilder();
        body.AppendLine("Las siguientes solicitudes están pendientes de revisión por más de 12 horas:");

        foreach (var request in pendingRequests)
        {
            body.AppendLine($"- Solicitud ID: {request.Id}, Tipo: {request.RequestTypeId}, Zona: {request.ZoneId}, Creada el: {request.CreatedDate}");
        }

        return body.ToString();
    }
}
