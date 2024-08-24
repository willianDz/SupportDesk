using Xunit;
using Moq;
using SupportDesk.Application.Contracts.Persistence;
using MimeKit;
using MailKit.Net.Smtp;
using SupportDesk.Application.Models.Notifications;
using SupportDesk.Infrastructure.Notifications;
using SupportDesk.Domain.Entities;
using MailKit;
using System.Net.Sockets;

namespace SupportDesk.Infrastructure.UnitTests.Notifications
{
    public class SmtpNotificationServiceTests
    {
        private readonly SmtpNotificationService _smtpNotificationService;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ISmtpClient> _smtpClientMock;

        public SmtpNotificationServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _smtpClientMock = new Mock<ISmtpClient>();

            _smtpNotificationService = new SmtpNotificationService(
                "smtp.testserver.com",
                587,
                "username",
                "password",
                _userRepositoryMock.Object,
                _smtpClientMock.Object);
        }

        [Fact]
        public async Task SendNotificationAsync_Should_Send_Email_Successfully()
        {
            // Arrange
            var userIds = new List<Guid> { Guid.NewGuid() };
            var users = new List<User>
            {
                new User { Id = userIds[0], Email = "testuser@test.com" }
            };

            _userRepositoryMock
                .Setup(repo => repo.GetUsersByIdsAsync(userIds, default))
                .ReturnsAsync(users);

            _smtpClientMock.Setup(client =>
                client.SendAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<ITransferProgress>()))
                .Returns(Task.FromResult(string.Empty));

            var notificationMessage = new NotificationMessage
            {
                RecipientUserIds = userIds,
                Subject = "Test Subject",
                Body = "Test Body"
            };

            // Act
            await _smtpNotificationService.SendNotificationAsync(notificationMessage);

            // Assert
            _smtpClientMock.Verify(client => client.SendAsync(
                It.Is<MimeMessage>(msg => msg.Subject == "Test Subject" && ((TextPart)msg.Body).Text == "Test Body" && msg.To.Mailboxes.Any(mb => mb.Address == "testuser@test.com")),
                It.IsAny<CancellationToken>(),
                It.IsAny<ITransferProgress>()),
                Times.Once);
        }

        [Fact]
        public async Task SendNotificationAsync_Should_Throw_Exception_On_Smtp_Failure()
        {
            // Arrange
            var userIds = new List<Guid> { Guid.NewGuid() };
            var users = new List<User>
            {
                new User { Id = userIds[0], Email = "testuser@test.com" }
            };

            _userRepositoryMock
                .Setup(repo => repo.GetUsersByIdsAsync(userIds, default))
                .ReturnsAsync(users);

            _smtpClientMock.Setup(client =>
                client.SendAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<ITransferProgress>()))
                .ThrowsAsync(new InvalidOperationException("SMTP Error"));

            var notificationMessage = new NotificationMessage
            {
                RecipientUserIds = userIds,
                Subject = "Test Subject",
                Body = "Test Body"
            };

            // Act & Assert
            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                _smtpNotificationService.SendNotificationAsync(notificationMessage));
        }

        [Fact]
        public void Constructor_Should_Throw_Exception_When_Smtp_Credentials_Are_Missing()
        {
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => new SmtpNotificationService(
                null!,
                587,
                "username",
                "password",
                _userRepositoryMock.Object,
                _smtpClientMock.Object));

            Assert.Throws<ArgumentNullException>(() => new SmtpNotificationService(
                "smtp.testserver.com",
                587,
                null!,
                "password",
                _userRepositoryMock.Object,
                _smtpClientMock.Object));
        }

        [Fact]
        public async Task SendNotificationAsync_Should_Retry_On_Transient_Failures()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var notificationMessage = new NotificationMessage
            {
                RecipientUserIds = new List<Guid> { userId },
                Subject = "Test Subject",
                Body = "Test Body"
            };

            var users = new List<User>
            {
                new User
                {
                    Id = userId,
                    Email = "test@example.com"
                }
            };

            // Configurar el mock del UserRepository para devolver una lista válida de usuarios
            _userRepositoryMock
                .Setup(repo => repo.GetUsersByIdsAsync(It.IsAny<List<Guid>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(users);

            _smtpClientMock
                .SetupSequence(client => client.SendAsync(
                    It.IsAny<MimeMessage>(),
                    It.IsAny<CancellationToken>(),
                    It.IsAny<ITransferProgress>()))
                .ThrowsAsync(new SocketException()) // Falla transitoria en el primer intento
                .ThrowsAsync(new SmtpCommandException(SmtpErrorCode.MessageNotAccepted, SmtpStatusCode.TransactionFailed, "Test Exception")) // Falla transitoria en el segundo intento
                .Returns(Task.FromResult(string.Empty)); // Éxito en el tercer intento

            // Act
            await _smtpNotificationService.SendNotificationAsync(notificationMessage, CancellationToken.None);

            // Assert
            _smtpClientMock.Verify(client => client.SendAsync(
                It.IsAny<MimeMessage>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<ITransferProgress>()),
            Times.Exactly(3));
        }
    }
}
