using Moq;
using Xunit;
using SupportDesk.Application.Features.Requests.Commands.CreateRequest;
using SupportDesk.Application.Contracts.Persistence;
using AutoMapper;
using SupportDesk.Domain.Entities;
using SupportDesk.Application.Models.Dtos;
using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.UnitTests.Features.Requests.Commands.CreateRequest;

public class CreateRequestCommandHandlerTests
{
    private readonly Mock<IRequestRepository> _requestRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly CreateRequestCommandHandler _handler;

    public CreateRequestCommandHandlerTests()
    {
        _requestRepositoryMock = new Mock<IRequestRepository>();
        _mapperMock = new Mock<IMapper>();
        _handler = new CreateRequestCommandHandler(_requestRepositoryMock.Object, _mapperMock.Object);
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
        Assert.Contains(result.ValidationErrors, e => e.Contains("Tipo de solicitud inválida"));
        Assert.Contains(result.ValidationErrors, e => e.Contains("Zona inválida"));
        Assert.Contains(result.ValidationErrors, e => e.Contains("Los comentarios deben tener al menos 15 caracteres."));
    }

}