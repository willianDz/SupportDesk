using Moq;
using SupportDesk.Application.Contracts.Persistence;
using Xunit;
using SupportDesk.Domain.Entities;
using SupportDesk.Application.Features.Requests.Commands.InactivateRequest;
using SupportDesk.Application.Constants;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Models.Notifications;
using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.UnitTests.Features.Requests.Commands.InactivateRequest;

public class InactivateRequestCommandHandlerTests
{
    private readonly Mock<IRequestRepository> _requestRepositoryMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<ILogger<InactivateRequestCommandHandler>> _loggerMock;
    private readonly InactivateRequestCommandHandler _handler;

    public InactivateRequestCommandHandlerTests()
    {
        _requestRepositoryMock = new Mock<IRequestRepository>();
        _notificationServiceMock = new Mock<INotificationService>();
        _loggerMock = new Mock<ILogger<InactivateRequestCommandHandler>>();

        _handler = new InactivateRequestCommandHandler(
            _requestRepositoryMock.Object,
            _notificationServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Not_Inactivate_Already_Inactive_Request()
    {
        // Arrange
        var command = new InactivateRequestCommand
        {
            RequestId = 1,
            UserId = Guid.NewGuid()
        };

        var existingRequest = new Request
        {
            Id = 1,
            IsActive = false
        };

        _requestRepositoryMock.Setup(r => r.GetByIdAsync(command.RequestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRequest);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(RequestMessages.RequestNotFoundOrIsInactive, result.Message);
        _requestRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()), Times.Never);
        _notificationServiceMock.Verify(n => n.SendNotificationAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()), Times.Never);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(NotificationsMessages.FailedToSendNotificationRequestInactivated)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Send_Notification_When_Request_Is_Inactivated_And_UnderReview()
    {
        // Arrange
        var command = new InactivateRequestCommand
        {
            RequestId = 1,
            UserId = Guid.NewGuid()
        };

        var existingRequest = new Request
        {
            Id = 1,
            IsActive = true,
            CreatedBy = command.UserId,
            RequestStatusId = (int)RequestStatusesEnum.UnderReview,
            ReviewerUserId = Guid.NewGuid()
        };

        _requestRepositoryMock.Setup(r => r.GetByIdAsync(command.RequestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRequest);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(RequestMessages.RequestHasBeenInactive, result.Message);
        _notificationServiceMock.Verify(n => n.SendNotificationAsync(It.Is<NotificationMessage>(m =>
            m.RecipientUserIds.Contains(existingRequest.ReviewerUserId.Value) &&
            m.Subject == "Una solicitud ha sido inactivada" &&
            m.Body.Contains($"La solicitud con ID: {existingRequest.Id} ha sido inactivada por el solicitante.")
        ), It.IsAny<CancellationToken>()), Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(NotificationsMessages.FailedToSendNotificationRequestInactivated)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Log_Error_When_Notification_Fails()
    {
        // Arrange
        var command = new InactivateRequestCommand
        {
            RequestId = 1,
            UserId = Guid.NewGuid()
        };

        var existingRequest = new Request
        {
            Id = 1,
            IsActive = true,
            CreatedBy = command.UserId,
            RequestStatusId = (int)RequestStatusesEnum.UnderReview,
            ReviewerUserId = Guid.NewGuid()
        };

        _requestRepositoryMock.Setup(r => r.GetByIdAsync(command.RequestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRequest);

        _notificationServiceMock.Setup(n => n.SendNotificationAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Failed to send email."));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(RequestMessages.RequestHasBeenInactive, result.Message);
        _notificationServiceMock.Verify(n => n.SendNotificationAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()), Times.Once);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(NotificationsMessages.FailedToSendNotificationRequestInactivated)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Not_Send_Notification_When_Request_Is_Inactivated_And_Not_UnderReview()
    {
        // Arrange
        var command = new InactivateRequestCommand
        {
            RequestId = 1,
            UserId = Guid.NewGuid()
        };

        var existingRequest = new Request
        {
            Id = 1,
            IsActive = true,
            CreatedBy = command.UserId,
            RequestStatusId = (int)RequestStatusesEnum.New
        };

        _requestRepositoryMock.Setup(r => r.GetByIdAsync(command.RequestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRequest);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal(RequestMessages.RequestHasBeenInactive, result.Message);

        _notificationServiceMock.Verify(n => n.SendNotificationAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()), Times.Never);

        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(NotificationsMessages.FailedToSendNotificationRequestInactivated)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Never);
    }
}