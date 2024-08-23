using Xunit;
using Moq;
using SupportDesk.Application.Contracts.Persistence;
using MimeKit;
using MailKit.Net.Smtp;
using SupportDesk.Application.Models.Notifications;
using SupportDesk.Infrastructure.Notifications;
using SupportDesk.Domain.Entities;
using MailKit;

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
    }
}
