using Moq;
using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using SupportDesk.Infrastructure.Notifications;
using Xunit;
using System.Net;
using FluentAssertions;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;

namespace SupportDesk.Infrastructure.UnitTests.Notifications
{
    public class SendGridNotificationServiceTests
    {
        private readonly Mock<ISendGridClient> _mockSendGridClient;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly SendGridNotificationService _notificationService;

        public SendGridNotificationServiceTests()
        {
            _mockSendGridClient = new Mock<ISendGridClient>();
            _mockConfiguration = new Mock<IConfiguration>();

            _mockConfiguration.Setup(c => c["SendGrid:ApiKey"]).Returns("FakeApiKey");
            _mockConfiguration.Setup(c => c["SendGrid:SenderEmail"]).Returns("sender@example.com");
            _mockConfiguration.Setup(c => c["SendGrid:SenderName"]).Returns("SupportDesk");

            _notificationService = new SendGridNotificationService(_mockSendGridClient.Object, _mockConfiguration.Object);
        }

        [Fact]
        public async Task SendEmailAsync_Should_Send_Email_Successfully()
        {
            // Arrange
            var emailNotification = new EmailNotification
            {
                To = new List<string> { "recipient@example.com" },
                Subject = "Test Email",
                Body = "This is a test email."
            };

            var sendGridResponse = new Response(HttpStatusCode.OK, null, null);

            _mockSendGridClient
                .Setup(client => client.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(sendGridResponse);

            // Act
            await _notificationService.SendEmailAsync(emailNotification);

            // Assert
            _mockSendGridClient.Verify(client => client.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SendEmailAsync_Should_Throw_Exception_On_Failure()
        {
            // Arrange
            var emailNotification = new EmailNotification
            {
                To = new List<string> { "recipient@example.com" },
                Subject = "Test Email",
                Body = "This is a test email."
            };

            var sendGridResponse = new Response(HttpStatusCode.BadRequest, null, null);

            _mockSendGridClient
                .Setup(client => client.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(sendGridResponse);

            // Act
            Func<Task> act = async () => await _notificationService.SendEmailAsync(emailNotification);

            // Assert
            await act.Should().ThrowAsync<Exception>().WithMessage("Failed to send email. Status code: BadRequest");

            _mockSendGridClient.Verify(client => client.SendEmailAsync(It.IsAny<SendGridMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void Constructor_Should_Throw_Exception_When_ApiKey_Is_Missing()
        {
            // Arrange
            _mockConfiguration.Setup(c => c["SendGrid:ApiKey"]).Returns((string)null);

            // Act
            Action act = () => new SendGridNotificationService(_mockSendGridClient.Object, _mockConfiguration.Object);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithMessage("Value cannot be null. (Parameter 'SendGrid API key is missing.')");
        }
    }
}
