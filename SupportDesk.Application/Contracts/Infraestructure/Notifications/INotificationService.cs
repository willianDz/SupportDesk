namespace SupportDesk.Application.Contracts.Infraestructure.Notifications;

public interface INotificationService
{
    Task SendEmailAsync(EmailNotification emailNotification, CancellationToken cancellationToken = default);
}

public class EmailNotification
{
    public List<string> To { get; set; } = new();
    public string Subject { get; set; } = string.Empty!;
    public string Body { get; set; } = string.Empty!;
    public bool IsHtml { get; set; } = true;
}