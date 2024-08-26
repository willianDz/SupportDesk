using Moq;
using Xunit;
using SupportDesk.Application.Features.Requests.Commands.CreateRequest;
using SupportDesk.Application.Contracts.Persistence;
using AutoMapper;
using SupportDesk.Domain.Entities;
using SupportDesk.Application.Models.Dtos;
using SupportDesk.Domain.Enums;
using SupportDesk.Application.Contracts.Infraestructure.FileStorage;
using Microsoft.AspNetCore.Http;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Models.Notifications;
using Microsoft.Extensions.Logging;

namespace SupportDesk.Application.UnitTests.Features.Requests.Commands.CreateRequest;

public class CreateRequestCommandHandlerTests
{
    private readonly Mock<IRequestRepository> _requestRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<CreateRequestCommandHandler>> _loggerMock;
    private readonly CreateRequestCommandHandler _handler;

    public CreateRequestCommandHandlerTests()
    {
        _requestRepositoryMock = new Mock<IRequestRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _notificationServiceMock = new Mock<INotificationService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<CreateRequestCommandHandler>>();

        _handler = new CreateRequestCommandHandler(
            _requestRepositoryMock.Object,
            _userRepositoryMock.Object,
            _fileStorageServiceMock.Object,
            _notificationServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_Request_And_Return_Response()
    {
        // Arrange
        var command = new CreateRequestCommand
        {
            UserId = Guid.NewGuid(),
            RequestTypeId = 1,
            ZoneId = 1,
            Comments = "Valid comments for the request"
        };

        var request = new Request
        {
            Id = 1,
            RequestTypeId = command.RequestTypeId,
            ZoneId = command.ZoneId,
            Comments = command.Comments,
            RequestStatusId = (int)RequestStatusesEnum.New
        };

        _requestRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        _mapperMock.Setup(m => m.Map<RequestDto>(It.IsAny<Request>())).Returns(new RequestDto { Id = 1 });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.RequestCreated);
        Assert.Equal(1, result.RequestCreated.Id);
        _requestRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_ValidationErrors_For_Invalid_Data()
    {
        // Arrange
        var command = new CreateRequestCommand
        {
            UserId = Guid.NewGuid(),
            RequestTypeId = 0, // Tipo de solicitud inválido
            ZoneId = 0, // Zona inválida
            Comments = "Short" // Comentarios muy cortos
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.ValidationErrors);
        Assert.NotEmpty(result.ValidationErrors);
        Assert.Contains(result.ValidationErrors, e => e.Contains(RequestMessages.InvalidRequestType));
        Assert.Contains(result.ValidationErrors, e => e.Contains(RequestMessages.InvalidZone));
        Assert.Contains(result.ValidationErrors, e => e.Contains(RequestMessages.CommentsMinLenght));
    }

    [Fact]
    public async Task Handle_Should_Call_FileStorageService_And_Save_Request_With_Documents()
    {
        // Arrange
        var command = new CreateRequestCommand
        {
            UserId = Guid.NewGuid(),
            RequestTypeId = 1,
            ZoneId = 1,
            Comments = "This is a valid comment",
            Documents = new List<IFormFile> { MockFormFile("doc1.jpg"), MockFormFile("doc2.jpg") }
        };

        _fileStorageServiceMock
            .Setup(x => x.UploadFilesAsync(It.IsAny<List<IFormFile>>(), "requests", CancellationToken.None))
            .ReturnsAsync(new List<string> { "http://localhost/files/requests/doc1.jpg", "http://localhost/files/requests/doc2.jpg" });

        var request = new Request
        {
            Id = 1,
            RequestTypeId = command.RequestTypeId,
            ZoneId = command.ZoneId,
            Comments = command.Comments,
            RequestStatusId = (int)RequestStatusesEnum.New,
            RequestDocuments = new List<RequestDocument>()
            {
                new RequestDocument()
                {
                    Id = 1,
                    RequestId = 1,
                    DocumentUrl = "http://localhost/files/requests/doc1.jpg",
                },
                new RequestDocument()
                {
                    Id = 2,
                    RequestId = 1,
                    DocumentUrl = "http://localhost/files/requests/doc2.jpg",
                }
            }
        };

        _requestRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        _mapperMock
            .Setup(m => m.Map<RequestDto>(It.IsAny<Request>()))
            .Returns(new RequestDto
            {
                Id = 1,
                RequestTypeId = command.RequestTypeId,
                ZoneId = command.ZoneId,
                Comments = command.Comments,
                RequestStatusId = (int)RequestStatusesEnum.New,
                RequestDocuments = new List<RequestDocumentDto>()
                {
                    new RequestDocumentDto()
                    {
                        Id = 1,
                        RequestId = 1,
                        DocumentUrl = "http://localhost/files/requests/doc1.jpg",
                    },
                    new RequestDocumentDto()
                    {
                        Id = 2,
                        RequestId = 1,
                        DocumentUrl = "http://localhost/files/requests/doc2.jpg",
                    }
                }
            });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _fileStorageServiceMock.Verify(x => x.UploadFilesAsync(command.Documents!, "requests", CancellationToken.None), Times.Once);

        _requestRepositoryMock.Verify(x => x.AddAsync(It.Is<Request>(r =>
            r.RequestDocuments != null &&
            r.RequestDocuments.Count == 2 &&
            r.RequestDocuments.Any(d => d.DocumentUrl == "http://localhost/files/requests/doc1.jpg") &&
            r.RequestDocuments.Any(d => d.DocumentUrl == "http://localhost/files/requests/doc2.jpg")
        ), CancellationToken.None), Times.Once);

        Assert.True(result.Success);
        Assert.NotNull(result.RequestCreated);
        Assert.Equal(2, result.RequestCreated.RequestDocuments?.Count);
    }

    [Fact]
    public async Task Handle_Should_Create_Request_And_Send_Notifications()
    {
        // Arrange
        var command = new CreateRequestCommand
        {
            UserId = Guid.NewGuid(),
            RequestTypeId = 1,
            ZoneId = 1,
            Comments = "Valid comments for the request",
            Documents = new List<IFormFile> { MockFormFile("doc1.jpg") }
        };

        var request = new Request
        {
            Id = 1,
            RequestTypeId = command.RequestTypeId,
            ZoneId = command.ZoneId,
            Comments = command.Comments,
            RequestStatusId = (int)RequestStatusesEnum.New
        };

        _requestRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        _fileStorageServiceMock
            .Setup(x => x.UploadFilesAsync(command.Documents!, "requests", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "http://localhost/files/requests/doc1.jpg" });

        _userRepositoryMock
            .Setup(u => u.GetSupervisorsAndAdminsForRequestAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "supervisor@test.com" },
                new User { Id = Guid.NewGuid(), Email = "admin@test.com" }
            });

        _mapperMock
            .Setup(m => m.Map<RequestDto>(It.IsAny<Request>()))
            .Returns(new RequestDto { Id = request.Id });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.RequestCreated);
        _requestRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()), Times.Once);
        _notificationServiceMock.Verify(n => n.SendNotificationAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Not_Send_Notifications_If_No_Supervisors_Or_Admins_Found()
    {
        // Arrange
        var command = new CreateRequestCommand
        {
            UserId = Guid.NewGuid(),
            RequestTypeId = 1,
            ZoneId = 1,
            Comments = "Valid comments for the request"
        };

        var request = new Request
        {
            Id = 1,
            RequestTypeId = command.RequestTypeId,
            ZoneId = command.ZoneId,
            Comments = command.Comments,
            RequestStatusId = (int)RequestStatusesEnum.New
        };

        _requestRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        _userRepositoryMock
            .Setup(u => u.GetSupervisorsAndAdminsForRequestAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>());

        _mapperMock
            .Setup(m => m.Map<RequestDto>(It.IsAny<Request>()))
            .Returns(new RequestDto { Id = request.Id });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.RequestCreated);
        _notificationServiceMock.Verify(n => n.SendNotificationAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Log_Error_When_Notification_Fails()
    {
        // Arrange
        var command = new CreateRequestCommand
        {
            UserId = Guid.NewGuid(),
            RequestTypeId = 1,
            ZoneId = 1,
            Comments = "Valid comments for the request"
        };

        var request = new Request
        {
            Id = 1,
            RequestTypeId = command.RequestTypeId,
            ZoneId = command.ZoneId,
            Comments = command.Comments,
            RequestStatusId = (int)RequestStatusesEnum.New
        };

        _requestRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        _userRepositoryMock
            .Setup(u => u.GetSupervisorsAndAdminsForRequestAsync(It.IsAny<int>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "supervisor@test.com" }
            });

        _mapperMock
            .Setup(m => m.Map<RequestDto>(It.IsAny<Request>()))
            .Returns(new RequestDto { Id = request.Id });

        _notificationServiceMock
            .Setup(n => n.SendNotificationAsync(It.IsAny<NotificationMessage>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("Notification failed"));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.RequestCreated);

        // Verificar que se llamó al método LogError
        _loggerMock.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(NotificationsMessages.FailedToSendNotificationRequestCreated)),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)),
            Times.Once);
    }

    private static IFormFile MockFormFile(string fileName)
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(100);

        return fileMock.Object;
    }
}