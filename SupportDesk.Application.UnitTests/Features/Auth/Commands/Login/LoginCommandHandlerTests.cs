using Moq;
using Xunit;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Contracts.Services;
using SupportDesk.Application.Features.Auth.Commands.Login;
using SupportDesk.Domain.Entities;
using Microsoft.Extensions.Logging;
using AutoMapper;
using SupportDesk.Application.Constants;

public class LoginCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<ITwoFactorService> _twoFactorServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<LoginCommandHandler>> _loggerMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordServiceMock = new Mock<IPasswordService>();
        _twoFactorServiceMock = new Mock<ITwoFactorService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<LoginCommandHandler>>();

        _handler = new LoginCommandHandler(
            _userRepositoryMock.Object,
            _passwordServiceMock.Object,
            _twoFactorServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Success_And_RequireTwoFactor_When_Credentials_Are_Valid()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "ValidPassword123"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            PasswordHash = "hashedPassword",
            IsActive = true
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordServiceMock.Setup(p => p.VerifyPassword(user.PasswordHash, command.Password))
            .Returns(true);

        _twoFactorServiceMock
            .Setup(t => t.GenerateAndSendTwoFactorCodeAsync(user.Id, user.Email, It.IsAny<CancellationToken>()))
            .Returns(Task.FromResult(string.Empty));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.True(result.RequiresTwoFactor);
        _twoFactorServiceMock.Verify(t => t.GenerateAndSendTwoFactorCodeAsync(user.Id, user.Email, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_InvalidCredentials_When_User_Not_Found()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "invalid@example.com",
            Password = "Password123"
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(AuthMessages.InvalidCredentials, result.Message);
    }

    [Fact]
    public async Task Handle_Should_Return_InvalidCredentials_When_Password_Is_Incorrect()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "InvalidPassword"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            PasswordHash = "hashedPassword",
            IsActive = true
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordServiceMock.Setup(p => p.VerifyPassword(user.PasswordHash, command.Password))
            .Returns(false);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(AuthMessages.InvalidCredentials, result.Message);
    }

    [Fact]
    public async Task Handle_Should_Return_UserIsInactive_When_User_Is_Inactive()
    {
        // Arrange
        var command = new LoginCommand
        {
            Email = "test@example.com",
            Password = "ValidPassword123"
        };

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            PasswordHash = "hashedPassword",
            IsActive = false
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(command.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _passwordServiceMock.Setup(p => p.VerifyPassword(user.PasswordHash, command.Password))
            .Returns(true);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(UsersMessages.UserIsInactive, result.Message);
    }
}
