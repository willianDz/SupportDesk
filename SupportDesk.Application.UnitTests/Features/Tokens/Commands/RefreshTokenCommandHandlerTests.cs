using Moq;
using Xunit;
using SupportDesk.Application.Contracts.Infraestructure.Security;
using SupportDesk.Application.Features.Tokens.Commands.RefreshToken;

namespace SupportDesk.Application.UnitTests.Features.Tokens.Commands;

public class RefreshTokenCommandHandlerTests
{
    private readonly Mock<ITokenGenerator> _tokenGeneratorMock;
    private readonly RefreshTokenCommandHandler _handler;

    public RefreshTokenCommandHandlerTests()
    {
        _tokenGeneratorMock = new Mock<ITokenGenerator>();
        _handler = new RefreshTokenCommandHandler(_tokenGeneratorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Call_RefreshToken_And_Return_New_Token()
    {
        // Arrange
        var command = new RefreshTokenCommand { Token = "validToken" };

        _tokenGeneratorMock.Setup(tg => tg.RefreshToken("validToken", CancellationToken.None))
                           .Returns("newValidToken");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("newValidToken", result);

        _tokenGeneratorMock
            .Verify(tg => tg.RefreshToken("validToken", CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Null_When_RefreshToken_Fails()
    {
        // Arrange
        var command = new RefreshTokenCommand { Token = "invalidToken" };

        _tokenGeneratorMock.Setup(tg => tg.RefreshToken("invalidToken", CancellationToken.None))
                           .Returns((string?)null); // Simulate failure

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
        _tokenGeneratorMock
            .Verify(tg => tg.RefreshToken("invalidToken", CancellationToken.None), Times.Once);
    }
}
