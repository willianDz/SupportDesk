using MailKit.Net.Smtp;
using MimeKit;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Models.Notifications;

namespace SupportDesk.Infrastructure.Notifications
{
    public class SmtpNotificationService : INotificationService
    {
        private readonly string _smtpServer;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly IUserRepository _userRepository;
        private readonly ISmtpClient _smtpClient;

        public SmtpNotificationService(
            string smtpServer,
            int smtpPort,
            string smtpUsername,
            string smtpPassword,
            IUserRepository userRepository,
            ISmtpClient smtpClient)
        {
            if (string.IsNullOrEmpty(smtpServer))
            {
                throw new ArgumentNullException(nameof(smtpServer));
            }

            if (smtpPort == 0)
            {
                throw new ArgumentNullException(nameof(smtpPort));
            }

            if (string.IsNullOrEmpty(smtpUsername))
            {
                throw new ArgumentNullException(nameof(smtpUsername));
            }

            if (string.IsNullOrEmpty(smtpPassword))
            {
                throw new ArgumentNullException(nameof(smtpPassword));
            }

            _smtpServer = smtpServer;
            _smtpPort = smtpPort;
            _smtpUsername = smtpUsername;
            _smtpPassword = smtpPassword;
            _userRepository = userRepository;
            _smtpClient = smtpClient;
        }

        public async Task SendNotificationAsync(
            NotificationMessage message,
            CancellationToken cancellationToken = default)
        {
            var recipientEmails = await GetRecipientEmailsAsync(
                message.RecipientUserIds,
                cancellationToken);

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("SupportDesk", _smtpUsername));
            emailMessage.To.AddRange(recipientEmails.Select(email => new MailboxAddress("", email)));
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart("html")
            {
                Text = message.Body
            };

            await _smtpClient.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
            await _smtpClient.AuthenticateAsync(_smtpUsername, _smtpPassword, cancellationToken);
            await _smtpClient.SendAsync(emailMessage, cancellationToken);
            await _smtpClient.DisconnectAsync(true, cancellationToken);
        }

        private async Task<List<string>> GetRecipientEmailsAsync(
            List<Guid> userIds,
            CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetUsersByIdsAsync(userIds, cancellationToken);
            return users
                .Select(user => user.Email)
                .Where(email => !string.IsNullOrWhiteSpace(email))
                .ToList();
        }
    }
}
