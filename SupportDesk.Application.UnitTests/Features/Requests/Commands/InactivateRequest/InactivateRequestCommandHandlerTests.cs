using Moq;
using SupportDesk.Application.Contracts.Persistence;
using Xunit;
using SupportDesk.Domain.Entities;
using SupportDesk.Application.Features.Requests.Commands.InactivateRequest;

namespace SupportDesk.Application.UnitTests.Features.Requests.Commands.InactivateRequest;

public class InactivateRequestCommandHandlerTests
{
    private readonly Mock<IRequestRepository> _requestRepositoryMock;
    private readonly InactivateRequestCommandHandler _handler;

    public InactivateRequestCommandHandlerTests()
    {
        _requestRepositoryMock = new Mock<IRequestRepository>();

        _handler = new InactivateRequestCommandHandler(_requestRepositoryMock.Object);
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
        Assert.Equal("La solicitud no existe o está inactiva.", result.Message);
        _requestRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()), Times.Never);
    }

}