using Moq;
using Xunit;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Models.Notifications;
using SupportDesk.Domain.Entities;
using SupportDesk.Application.Features.Alerts;
using SupportDesk.Application.Constants;

namespace SupportDesk.Application.UnitTests.Features.Alerts;

public class PendingRequestsAlertServiceTests
{
    private readonly Mock<IRequestRepository> _requestRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<ILogger<PendingRequestsAlertService>> _loggerMock;
    private readonly PendingRequestsAlertService _pendingRequestsAlertService;

    public PendingRequestsAlertServiceTests()
    {
        _requestRepositoryMock = new Mock<IRequestRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _notificationServiceMock = new Mock<INotificationService>();
        _loggerMock = new Mock<ILogger<PendingRequestsAlertService>>();

        _pendingRequestsAlertService = new PendingRequestsAlertService(
            _requestRepositoryMock.Object,
            _userRepositoryMock.Object,
            _notificationServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task SendPendingRequestsAlertAsync_Should_Not_Send_Notification_When_No_Pending_Requests()
    {
        // Arrange
        _requestRepositoryMock.Setup(repo => repo.GetPendingRequestsAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Request>());

        // Act
        await _pendingRequestsAlertService.SendPendingRequestsAlertAsync();

        // Assert
        _notificationServiceMock.Verify(ns => ns.SendNotificationAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task SendPendingRequestsAlertAsync_Should_Send_Notifications_To_Supervisors()
    {
        // Arrange
        var pendingRequests = new List<Request>
    {
        new Request { Id = 1, ZoneId = 1, RequestTypeId = 1, CreatedDate = DateTime.UtcNow.AddHours(-13) },
        new Request { Id = 2, ZoneId = 1, RequestTypeId = 1, CreatedDate = DateTime.UtcNow.AddHours(-14) }
    };

        _requestRepositoryMock.Setup(repo => repo.GetPendingRequestsAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingRequests);

        var supervisors = new List<User>
    {
        new User { Id = Guid.NewGuid(), IsSupervisor = true }
    };

        _userRepositoryMock.Setup(repo => repo.GetSupervisorsByZoneAndRequestTypeAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(supervisors);

        // Act
        await _pendingRequestsAlertService.SendPendingRequestsAlertAsync();

        // Assert
        _notificationServiceMock.Verify(ns => ns.SendNotificationAsync(It.Is<NotificationMessage>(nm =>
            nm.RecipientUserIds.SequenceEqual(supervisors.Select(s => s.Id)) &&
            nm.Subject == "Alerta: Solicitudes pendientes de revisión" &&
            nm.Body.Contains("Solicitud ID: 1") &&
            nm.Body.Contains("Solicitud ID: 2")),
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task SendPendingRequestsAlertAsync_Should_Log_Error_On_Exception()
    {
        // Arrange
        _requestRepositoryMock.Setup(repo => repo.GetPendingRequestsAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Test Exception"));

        // Act
        await _pendingRequestsAlertService.SendPendingRequestsAlertAsync();

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(AlertsMessages.FailedToSendPendingRequestsAlert)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task SendPendingRequestsAlertAsync_Should_Not_Send_Notification_When_No_Supervisors()
    {
        // Arrange
        var pendingRequests = new List<Request>
    {
        new Request { Id = 1, ZoneId = 1, RequestTypeId = 1, CreatedDate = DateTime.UtcNow.AddHours(-13) }
    };

        _requestRepositoryMock.Setup(repo => repo.GetPendingRequestsAsync(It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(pendingRequests);

        _userRepositoryMock.Setup(repo => repo.GetSupervisorsByZoneAndRequestTypeAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>()); // No supervisors found

        // Act
        await _pendingRequestsAlertService.SendPendingRequestsAlertAsync();

        // Assert
        _notificationServiceMock.Verify(ns => ns.SendNotificationAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}