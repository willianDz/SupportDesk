using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;

namespace SupportDesk.Infrastructure.Notifications
{
    public class SendGridNotificationService : INotificationService
    {
        private readonly ISendGridClient _client;
        private readonly IConfiguration _configuration;
        private readonly string _sendGridApiKey;

        public SendGridNotificationService(ISendGridClient client, IConfiguration configuration)
        {
            _client = client;
            _configuration = configuration;
            _sendGridApiKey = _configuration["SendGrid:ApiKey"] ?? throw new ArgumentNullException("SendGrid API key is missing.");
        }

        public async Task SendEmailAsync(EmailNotification emailNotification, CancellationToken cancellationToken = default)
        {
            var from = new EmailAddress(_configuration["SendGrid:SenderEmail"], _configuration["SendGrid:SenderName"]);

            var msg = MailHelper.CreateSingleEmailToMultipleRecipients(
                from,
                emailNotification.To.Select(to => new EmailAddress(to)).ToList(),
                emailNotification.Subject,
                emailNotification.Body,
                emailNotification.IsHtml ? emailNotification.Body : null
            );

            var response = await _client.SendEmailAsync(msg, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to send email. Status code: {response.StatusCode}");
            }
        }
    }
}
