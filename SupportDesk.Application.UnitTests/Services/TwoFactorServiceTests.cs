using Moq;
using SupportDesk.Application.Contracts.Infraestructure.Notifications;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Models.Notifications;
using SupportDesk.Application.Services;
using Xunit;

public class TwoFactorServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<INotificationService> _notificationServiceMock;
    private readonly TwoFactorService _twoFactorService;

    public TwoFactorServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _notificationServiceMock = new Mock<INotificationService>();
        _twoFactorService = new TwoFactorService(_userRepositoryMock.Object, _notificationServiceMock.Object);
    }

    [Fact]
    public async Task GenerateAndSendTwoFactorCodeAsync_Should_Generate_And_Send_Code()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";

        // Act
        var code = await _twoFactorService.GenerateAndSendTwoFactorCodeAsync(userId, email, CancellationToken.None);

        // Assert
        Assert.NotNull(code);
        Assert.Equal(6, code.Length);

        _userRepositoryMock.Verify(repo => repo.SaveTwoFactorCodeAsync(userId, code, It.IsAny<CancellationToken>()), Times.Once);
        _notificationServiceMock.Verify(service => service.SendNotificationAsync(It.Is<NotificationMessage>(m =>
            m.RecipientUserIds.Contains(userId) &&
            m.Body.Contains(code)
        ), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ValidateTwoFactorCodeAsync_Should_Return_True_When_Code_Is_Valid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var validCode = "123456";

        _userRepositoryMock.Setup(repo => repo.ValidateTwoFactorCodeAsync(userId, validCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        var isValid = await _twoFactorService.ValidateTwoFactorCodeAsync(userId, validCode, CancellationToken.None);

        // Assert
        Assert.True(isValid);
    }

    [Fact]
    public async Task ValidateTwoFactorCodeAsync_Should_Return_False_When_Code_Is_Invalid()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var invalidCode = "654321";

        _userRepositoryMock.Setup(repo => repo.ValidateTwoFactorCodeAsync(userId, invalidCode, It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        // Act
        var isValid = await _twoFactorService.ValidateTwoFactorCodeAsync(userId, invalidCode, CancellationToken.None);

        // Assert
        Assert.False(isValid);
    }
}
