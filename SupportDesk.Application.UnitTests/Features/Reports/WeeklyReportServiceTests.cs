using Moq;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Features.Reports;
using SupportDesk.Application.Models.Notifications;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;
using Xunit;
using Microsoft.Extensions.Logging;
using System.Text;
using SupportDesk.Application.Constants;

namespace SupportDesk.Application.UnitTests.Features.Reports
{
    public class WeeklyReportServiceTests
    {
        private readonly Mock<IRequestRepository> _requestRepositoryMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly Mock<ILogger<WeeklyReportService>> _loggerMock;
        private readonly WeeklyReportService _weeklyReportService;

        public WeeklyReportServiceTests()
        {
            _requestRepositoryMock = new Mock<IRequestRepository>();
            _userRepositoryMock = new Mock<IUserRepository>();
            _notificationServiceMock = new Mock<INotificationService>();
            _loggerMock = new Mock<ILogger<WeeklyReportService>>();

            _weeklyReportService = new WeeklyReportService(
                _requestRepositoryMock.Object,
                _userRepositoryMock.Object,
                _notificationServiceMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task SendWeeklyReportAsync_Should_Send_Report_When_Data_Exists()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-7);
            var endDate = DateTime.UtcNow;

            _requestRepositoryMock.Setup(repo => repo.GetRequestCountByStatusAsync(It.IsAny<int>(), startDate, endDate, default))
                .ReturnsAsync(10);

            _requestRepositoryMock.Setup(repo => repo.GetWeeklyRequestCountsAsync(startDate, endDate, default))
                .ReturnsAsync(new Dictionary<int, int> { { (int)RequestStatusesEnum.New, 10 } });

            _requestRepositoryMock.Setup(repo => repo.GetWeeklyRequestCountsAsync(startDate.AddDays(-7), endDate.AddDays(-7), default))
                .ReturnsAsync(new Dictionary<int, int> { { (int)RequestStatusesEnum.New, 8 } });

            _requestRepositoryMock.Setup(repo => repo.GetRequestTrendsByTypeAndZoneAsync(startDate, endDate, default))
                .ReturnsAsync(new List<(int RequestTypeId, int ZoneId, int Count)> { (1, 1, 5) });

            _requestRepositoryMock.Setup(repo => repo.GetAverageResolutionTimeAsync(startDate, endDate, default))
                .ReturnsAsync(TimeSpan.FromHours(5));

            _userRepositoryMock.Setup(repo => repo.GetAdminUsersAsync(default))
                .ReturnsAsync(new List<User> { new User { Id = Guid.NewGuid(), Email = "admin@example.com" } });

            // Act
            await _weeklyReportService.SendWeeklyReportAsync();

            // Assert
            _notificationServiceMock.Verify(n => n.SendNotificationAsync(It.IsAny<NotificationMessage>(), default), Times.Once);
        }

        [Fact]
        public async Task SendWeeklyReportAsync_Should_Log_Error_When_Exception_Occurs()
        {
            // Arrange
            _requestRepositoryMock.Setup(repo => repo.GetRequestCountByStatusAsync(It.IsAny<int>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), default))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            await _weeklyReportService.SendWeeklyReportAsync();

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(ReportsMessages.FailedToSendWeeklyReport)),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
                Times.Once);

            _notificationServiceMock.Verify(n => n.SendNotificationAsync(It.IsAny<NotificationMessage>(), default), Times.Never);
        }
    }
}
