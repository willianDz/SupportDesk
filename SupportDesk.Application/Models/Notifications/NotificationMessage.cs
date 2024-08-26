namespace SupportDesk.Application.Models.Notifications;

public class NotificationMessage
{
    public List<Guid> RecipientUserIds { get; set; } = new();
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
