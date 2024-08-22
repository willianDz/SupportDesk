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

namespace SupportDesk.Application.UnitTests.Features.Requests.Commands.UpdateRequest;

public class UpdateRequestCommandHandlerTests
{
    private readonly Mock<IRequestRepository> _requestRepositoryMock;
    private readonly Mock<IRequestValidationService> _requestValidationServiceMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UpdateRequestCommandHandler _handler;

    public UpdateRequestCommandHandlerTests()
    {
        _requestRepositoryMock = new Mock<IRequestRepository>();
        _requestValidationServiceMock = new Mock<IRequestValidationService>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _mapperMock = new Mock<IMapper>();

        _handler = new UpdateRequestCommandHandler(
            _requestRepositoryMock.Object,
            _requestValidationServiceMock.Object,
            _fileStorageServiceMock.Object,
            _mapperMock.Object);
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
}