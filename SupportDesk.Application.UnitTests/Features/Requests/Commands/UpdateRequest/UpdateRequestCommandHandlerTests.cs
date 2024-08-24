using Moq;
using Xunit;
using SupportDesk.Application.Features.Requests.Commands.UpdateRequest;
using SupportDesk.Application.Contracts.Persistence;
using AutoMapper;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;
using SupportDesk.Application.Contracts.Infraestructure.FileStorage;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Services;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Models.Notifications;

namespace SupportDesk.Application.UnitTests.Features.Requests.Commands.UpdateRequest;

public class UpdateRequestCommandHandlerTests
{
    private readonly Mock<IRequestRepository> _requestRepositoryMock;
    private readonly Mock<IRequestValidationService> _requestValidationServiceMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<UpdateRequestCommandHandler>> _loggerMock;
    private readonly UpdateRequestCommandHandler _handler;

    public UpdateRequestCommandHandlerTests()
    {
        _requestRepositoryMock = new Mock<IRequestRepository>();
        _requestValidationServiceMock = new Mock<IRequestValidationService>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _notificationServiceMock = new Mock<INotificationService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<UpdateRequestCommandHandler>>();

        _handler = new UpdateRequestCommandHandler(
            _requestRepositoryMock.Object,
            _requestValidationServiceMock.Object,
            _fileStorageServiceMock.Object,
            _notificationServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Not_Update_Request_In_Approved_State()
    {
        // Arrange
        var command = new UpdateRequestCommand 
        { 
            RequestId = 1, 
            RequestTypeId = 2, 
            ZoneId = 1, 
            Comments = "Updated comments" 
        };

        var existingRequest = new Request
        {
            Id = 1,
            RequestStatusId = (int)RequestStatusesEnum.Approved,
            IsActive = true
        };

        _requestRepositoryMock.Setup(r => r.GetByIdAsync(command.RequestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRequest);

        _requestValidationServiceMock.Setup(v => v.ValidateUserCanUpdateHisRequestAsync(It.IsAny<Request>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(RequestMessages.RequestAlreadyProccessed));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(RequestMessages.RequestAlreadyProccessed, result.Message);
        _requestRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()), Times.Never);
    }


    [Fact]
    public async Task Handle_Should_Send_Notification_When_Request_In_Review_Is_Updated()
    {
        // Arrange
        var command = new UpdateRequestCommand
        {
            RequestId = 1,
            RequestTypeId = 2,
            ZoneId = 1,
            Comments = "Valid Updated comments for the request"
        };

        var existingRequest = new Request
        {
            Id = 1,
            RequestStatusId = (int)RequestStatusesEnum.UnderReview,
            IsActive = true,
            ReviewerUserId = Guid.NewGuid()
        };

        _requestRepositoryMock.Setup(r => r.GetByIdAsync(command.RequestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRequest);

        _requestValidationServiceMock.Setup(v => v.ValidateUserCanUpdateHisRequestAsync(It.IsAny<Request>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _notificationServiceMock.Verify(ns => ns.SendNotificationAsync(It.Is<NotificationMessage>(nm =>
            nm.RecipientUserIds.Contains(existingRequest.ReviewerUserId.Value) &&
            nm.Subject == "Solicitud actualizada por el solicitante" &&
            nm.Body.Contains(existingRequest.Id.ToString())
        ), It.IsAny<CancellationToken>()), Times.Once);

        _requestRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task Handle_Should_Not_Send_Notification_If_Request_Not_In_Review()
    {
        // Arrange
        var command = new UpdateRequestCommand
        {
            RequestId = 1,
            RequestTypeId = 2,
            ZoneId = 1,
            Comments = "Valid updated comments for the request"
        };

        var existingRequest = new Request
        {
            Id = 1,
            RequestStatusId = (int)RequestStatusesEnum.New,
            IsActive = true
        };

        _requestRepositoryMock.Setup(r => r.GetByIdAsync(command.RequestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRequest);

        _requestValidationServiceMock.Setup(v => v.ValidateUserCanUpdateHisRequestAsync(It.IsAny<Request>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _notificationServiceMock.Verify(ns => ns.SendNotificationAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()), Times.Never);
        Assert.True(result.Success);
    }

    [Fact]
    public async Task Handle_Should_Log_Error_When_Notification_Fails()
    {
        // Arrange
        var command = new UpdateRequestCommand
        {
            RequestId = 1,
            RequestTypeId = 2,
            ZoneId = 1,
            Comments = "Valid updated comments for the request"
        };

        var existingRequest = new Request
        {
            Id = 1,
            RequestStatusId = (int)RequestStatusesEnum.UnderReview,
            IsActive = true,
            ReviewerUserId = Guid.NewGuid()
        };

        _requestRepositoryMock.Setup(r => r.GetByIdAsync(command.RequestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingRequest);

        _requestValidationServiceMock.Setup(v => v.ValidateUserCanUpdateHisRequestAsync(It.IsAny<Request>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(true));

        _notificationServiceMock.Setup(ns => ns.SendNotificationAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Notification error"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(NotificationsMessages.FailedToSendNotificationRequestUpdate)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);

        _requestRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()), Times.Once);
        
        Assert.True(result.Success);
    }
}