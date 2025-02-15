﻿using MailKit.Net.Smtp;
using MimeKit;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Models.Notifications;
using Polly.Retry;
using Polly;
using System.Net.Sockets;

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
        private readonly AsyncRetryPolicy _retryPolicy;

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

            // Configurar la política de reintento con Polly
            _retryPolicy = Policy
                .Handle<SocketException>() // Manejar errores transitorios de red
                .Or<SmtpCommandException>() // Manejar errores del servidor SMTP
                .Or<SmtpProtocolException>() // Manejar errores del protocolo SMTP
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                    {
                        // registrar logs
                    });
        }

        public async Task SendNotificationAsync(
            NotificationMessage message,
            CancellationToken cancellationToken = default)
        {
            var recipientEmails = await GetRecipientEmailsAsync(
                message.RecipientUserIds,
                cancellationToken);

            if (recipientEmails is null)
            {
                return;
            }

            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("SupportDesk", _smtpUsername));
            emailMessage.To.AddRange(recipientEmails.Select(email => new MailboxAddress("", email)));
            emailMessage.Subject = message.Subject;
            emailMessage.Body = new TextPart("html")
            {
                Text = message.Body
            };

            await _retryPolicy.ExecuteAsync(async () =>
            {
                await _smtpClient.ConnectAsync(_smtpServer, _smtpPort, MailKit.Security.SecureSocketOptions.StartTls, cancellationToken);
                await _smtpClient.AuthenticateAsync(_smtpUsername, _smtpPassword, cancellationToken);
                await _smtpClient.SendAsync(emailMessage, cancellationToken);
                await _smtpClient.DisconnectAsync(true, cancellationToken);
            });
        }

        private async Task<List<string>?> GetRecipientEmailsAsync(
            List<Guid> userIds,
            CancellationToken cancellationToken)
        {
            var users = await _userRepository.GetUsersByIdsAsync(userIds, cancellationToken);
            return users?
                .Select(user => user.Email)
                .Where(email => !string.IsNullOrWhiteSpace(email))
                .ToList();
        }
    }
}
