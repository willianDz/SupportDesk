using Microsoft.Extensions.Logging;
using Moq;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Features.Reports;
using SupportDesk.Application.Models.Notifications;
using SupportDesk.Domain.Entities;
using Xunit;

namespace SupportDesk.Application.UnitTests.Features.Reports
{
    public class DailyReportServiceTests
    {
        private readonly Mock<IRequestRepository> _requestRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<ILogger<DailyReportService>> _loggerMock;
        private readonly DailyReportService _dailyReportService;

        public DailyReportServiceTests()
        {
            _requestRepositoryMock = new Mock<IRequestRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _notificationServiceMock = new Mock<INotificationService>();
            _loggerMock = new Mock<ILogger<DailyReportService>>();

            _dailyReportService = new DailyReportService(
                _requestRepositoryMock.Object,
                _userRepositoryMock.Object,
                _notificationServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task SendDailyReportAsync_Should_Send_Report_When_There_Are_Admins()
        {
            // Arrange
            _requestRepositoryMock.Setup(r => r.GetRequestsCountByDateAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(10);
            _requestRepositoryMock.Setup(r => r.GetProcessedRequestsCountByDateAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(5);
            _requestRepositoryMock.Setup(r => r.GetPendingRequestsCountAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(3);
            _requestRepositoryMock.Setup(r => r.GetAverageResponseTimeAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(TimeSpan.FromHours(2));

            var adminUsers = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "admin@example.com" }
            };

            _userRepositoryMock.Setup(u => u.GetAdminUsersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(adminUsers);

            // Act
            await _dailyReportService.SendDailyReportAsync();

            // Assert
            _notificationServiceMock.Verify(n => n.SendNotificationAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task SendDailyReportAsync_Should_Not_Send_Report_When_There_Are_No_Admins()
        {
            // Arrange
            _userRepositoryMock.Setup(u => u.GetAdminUsersAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<User>());

            // Act
            await _dailyReportService.SendDailyReportAsync();

            // Assert
            _notificationServiceMock.Verify(n => n.SendNotificationAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task SendDailyReportAsync_Should_Log_Error_When_Exception_Is_Thrown()
        {
            // Arrange
            _requestRepositoryMock.Setup(r => r.GetRequestsCountByDateAsync(It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            await _dailyReportService.SendDailyReportAsync();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(ReportsMessages.FailedToSendDailyReport)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);
        }
    }
}
