using Moq;
using Xunit;
using SupportDesk.Application.Features.Auth.Commands.VerifyTwoFactor;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Contracts.Infraestructure.Security;
using SupportDesk.Domain.Entities;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Constants;

public class VerifyTwoFactorCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ITokenGenerator> _tokenGeneratorMock;
    private readonly Mock<ILogger<VerifyTwoFactorCommandHandler>> _loggerMock;
    private readonly VerifyTwoFactorCommandHandler _handler;

    public VerifyTwoFactorCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _tokenGeneratorMock = new Mock<ITokenGenerator>();
        _loggerMock = new Mock<ILogger<VerifyTwoFactorCommandHandler>>();

        _handler = new VerifyTwoFactorCommandHandler(
            _userRepositoryMock.Object,
            _tokenGeneratorMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_JwtToken_When_TwoFactorCode_Is_Valid()
    {
        // Arrange
        var command = new VerifyTwoFactorCommand { Email = "test@example.com", TwoFactorCode = "123456" };
        var user = new User { Email = command.Email, Id = Guid.NewGuid(), IsAdmin = true, IsSupervisor = false };

        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(repo => repo.ValidateTwoFactorCodeAsync(user.Id, command.TwoFactorCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _tokenGeneratorMock.Setup(service => service.GenerateToken(It.IsAny<TokenGenerationRequest>(), It.IsAny<CancellationToken>()))
            .Returns("jwtToken");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.Equal("jwtToken", result.JwtToken);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_TwoFactorCode_Is_Invalid()
    {
        // Arrange
        var command = new VerifyTwoFactorCommand { Email = "test@example.com", TwoFactorCode = "WrongCode" };

        _userRepositoryMock.Setup(repo => repo.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Email = command.Email });

        _userRepositoryMock.Setup(repo => repo.ValidateTwoFactorCodeAsync(It.IsAny<Guid>(), command.TwoFactorCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(AuthMessages.InvalidTwoFactorCode, result.Message);
    }
}
