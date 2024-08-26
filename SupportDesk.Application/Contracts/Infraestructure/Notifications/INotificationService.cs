using SupportDesk.Application.Models.Notifications;

namespace SupportDesk.Application.Contracts.Infraestructure.Notifications;

public interface INotificationService
{
    Task SendNotificationAsync(
        NotificationMessage message,
        CancellationToken cancellationToken = default);
}