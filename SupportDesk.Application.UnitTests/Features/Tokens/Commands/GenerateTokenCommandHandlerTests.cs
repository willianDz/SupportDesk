using Moq;
using Xunit;
using SupportDesk.Application.Contracts.Infraestructure.Security;
using SupportDesk.Application.Features.Tokens.Commands.GenerateToken;

namespace SupportDesk.Application.UnitTests.Features.Tokens.Commands;

public class GenerateTokenCommandHandlerTests
{
    private readonly Mock<ITokenGenerator> _tokenGeneratorMock;
    private readonly GenerateTokenCommandHandler _handler;

    public GenerateTokenCommandHandlerTests()
    {
        _tokenGeneratorMock = new Mock<ITokenGenerator>();
        _handler = new GenerateTokenCommandHandler(_tokenGeneratorMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Call_GenerateToken_And_Return_Token()
    {
        // Arrange
        var command = new GenerateTokenCommand
        {
            UserId = Guid.NewGuid(),
            Email = "testuser@test.com",
            IsSupervisor = true,
            IsAdmin = false,
            CustomClaims = []
        };

        _tokenGeneratorMock.Setup(tg => tg.GenerateToken(It.IsAny<TokenGenerationRequest>(), CancellationToken.None))
                           .Returns("generatedToken");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal("generatedToken", result);

        _tokenGeneratorMock.Verify(tg => tg.GenerateToken(It.Is<TokenGenerationRequest>(
            r => r.UserId == command.UserId &&
                 r.Email == command.Email &&
                 r.IsSupervisor == command.IsSupervisor &&
                 r.IsAdmin == command.IsAdmin), CancellationToken.None), Times.Once);
    }
}
