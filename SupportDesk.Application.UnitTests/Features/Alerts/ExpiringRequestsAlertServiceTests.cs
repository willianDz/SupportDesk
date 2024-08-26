using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Models.Notifications;
using SupportDesk.Domain.Entities;
using SupportDesk.Application.Features.Alerts;
using SupportDesk.Application.Constants;

public class ExpiringRequestsAlertServiceTests
{
    private readonly Mock<IRequestRepository> _requestRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<ILogger<ExpiringRequestsAlertService>> _loggerMock;
    private readonly ExpiringRequestsAlertService _expiringRequestsAlertService;

    public ExpiringRequestsAlertServiceTests()
    {
        _requestRepositoryMock = new Mock<IRequestRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _notificationServiceMock = new Mock<INotificationService>();
        _loggerMock = new Mock<ILogger<ExpiringRequestsAlertService>>();

        _expiringRequestsAlertService = new ExpiringRequestsAlertService(
            _requestRepositoryMock.Object,
            _userRepositoryMock.Object,
            _notificationServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task SendExpiringRequestsAlertAsync_Should_Not_Send_Notification_When_No_Expiring_Requests()
    {
        // Arrange
        _requestRepositoryMock.Setup(repo => repo.GetExpiringRequestsAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Request>());

        // Act
        await _expiringRequestsAlertService.SendExpiringRequestsAlertAsync();

        // Assert
        _notificationServiceMock.Verify(ns => ns.SendNotificationAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SendExpiringRequestsAlertAsync_Should_Send_Notifications_To_Admins()
    {
        // Arrange
        var expiringRequests = new List<Request>
        {
            new Request { Id = 1, ZoneId = 1, RequestTypeId = 1, CreatedDate = DateTime.UtcNow.AddHours(-21) },
            new Request { Id = 2, ZoneId = 1, RequestTypeId = 1, CreatedDate = DateTime.UtcNow.AddHours(-22) }
        };

        _requestRepositoryMock.Setup(repo => repo.GetExpiringRequestsAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expiringRequests);

        var adminUsers = new List<User>
        {
            new User { Id = Guid.NewGuid(), IsAdmin = true }
        };

        _userRepositoryMock.Setup(repo => repo.GetAdminUsersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(adminUsers);

        // Act
        await _expiringRequestsAlertService.SendExpiringRequestsAlertAsync();

        // Assert
        _notificationServiceMock.Verify(ns => ns.SendNotificationAsync(It.Is<NotificationMessage>(nm =>
            nm.RecipientUserIds.SequenceEqual(adminUsers.Select(s => s.Id)) &&
            nm.Subject == "Alerta: Solicitudes Próximas a Expirar" &&
            nm.Body.Contains("Solicitud ID: 1") &&
            nm.Body.Contains("Solicitud ID: 2")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendExpiringRequestsAlertAsync_Should_Log_Error_On_Exception()
    {
        // Arrange
        _requestRepositoryMock.Setup(repo => repo.GetExpiringRequestsAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test Exception"));

        // Act
        await _expiringRequestsAlertService.SendExpiringRequestsAlertAsync();

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(AlertsMessages.FailedToSendExpiringRequestsAlert)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task SendExpiringRequestsAlertAsync_Should_Not_Send_Notification_When_No_Admins()
    {
        // Arrange
        var expiringRequests = new List<Request>
        {
            new Request { Id = 1, ZoneId = 1, RequestTypeId = 1, CreatedDate = DateTime.UtcNow.AddHours(-21) }
        };

        _requestRepositoryMock.Setup(repo => repo.GetExpiringRequestsAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expiringRequests);

        _userRepositoryMock.Setup(repo => repo.GetAdminUsersAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>()); // No admins found

        // Act
        await _expiringRequestsAlertService.SendExpiringRequestsAlertAsync();

        // Assert
        _notificationServiceMock.Verify(ns => ns.SendNotificationAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}
