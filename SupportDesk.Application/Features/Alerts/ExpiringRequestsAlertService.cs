using Microsoft.Extensions.Logging;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Models.Notifications;
using SupportDesk.Domain.Entities;
using System.Text;

namespace SupportDesk.Application.Features.Alerts;

public class ExpiringRequestsAlertService : IExpiringRequestsAlertService
{
    private readonly IRequestRepository _requestRepository;
    private readonly IUserRepository _userRepository;
    private readonly INotificationService _notificationService;
    private readonly ILogger<ExpiringRequestsAlertService> _logger;

    public ExpiringRequestsAlertService(
        IRequestRepository requestRepository,
        IUserRepository userRepository,
        INotificationService notificationService,
        ILogger<ExpiringRequestsAlertService> logger)
    {
        _requestRepository = requestRepository;
        _userRepository = userRepository;
        _notificationService = notificationService;
        _logger = logger;
    }

    public async Task SendExpiringRequestsAlertAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var expiringThreshold = TimeSpan.FromHours(20); // Más de 20 horas
            var expiringRequests = await _requestRepository.GetExpiringRequestsAsync(expiringThreshold, cancellationToken);

            if (expiringRequests.Count == 0)
            {
                return;
            }

            var adminUsers = await _userRepository.GetAdminUsersAsync(cancellationToken);

            if (adminUsers is null || adminUsers.Count == 0)
            {
                return;
            }

            var notificationMessage = new NotificationMessage
            {
                RecipientUserIds = adminUsers.Select(u => u.Id).ToList(),
                Subject = "Alerta: Solicitudes Próximas a Expirar",
                Body = BuildAlertBody(expiringRequests)
            };

            await _notificationService.SendNotificationAsync(notificationMessage, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, AlertsMessages.FailedToSendExpiringRequestsAlert);
        }
    }

    private string BuildAlertBody(IReadOnlyList<Request> expiringRequests)
    {
        var body = new StringBuilder();
        body.AppendLine("Las siguientes solicitudes están próximas a expirar:");

        foreach (var request in expiringRequests)
        {
            var reviewer = request.ReviewerUserId.HasValue ? $"Revisada por: {request.ReviewerUserId}" : "No revisada";
            var expirationTime = request.CreatedDate.AddHours(24);
            var timeRemaining = expirationTime - DateTime.UtcNow;

            body.AppendLine($"- Solicitud ID: {request.Id}, Tipo: {request.RequestTypeId}, Creada el: {request.CreatedDate}, " +
                            $"{reviewer}, Fecha límite: {expirationTime}, Tiempo restante: {timeRemaining}");
        }

        return body.ToString();
    }
}
