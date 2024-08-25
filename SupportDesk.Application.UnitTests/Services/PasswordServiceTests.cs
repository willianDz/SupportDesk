using SupportDesk.Application.Contracts.Services;
using SupportDesk.Application.Services;
using Xunit;

namespace SupportDesk.Application.UnitTests.Services;

public class PasswordServiceTests
{
    private readonly IPasswordService _passwordService;

    public PasswordServiceTests()
    {
        _passwordService = new PasswordService();
    }

    [Fact]
    public void HashPassword_Should_Return_Hashed_Password()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hashedPassword = _passwordService.HashPassword(password);

        // Assert
        Assert.NotNull(hashedPassword);
        Assert.NotEqual(password, hashedPassword);
    }

    [Fact]
    public void VerifyPassword_Should_Return_True_For_Correct_Password()
    {
        // Arrange
        var password = "TestPassword123!";
        var hashedPassword = _passwordService.HashPassword(password);

        // Act
        var result = _passwordService.VerifyPassword(hashedPassword, password);

        // Assert
        Assert.True(result);
    }

    [Fact]
    public void VerifyPassword_Should_Return_False_For_Incorrect_Password()
    {
        // Arrange
        var password = "TestPassword123!";
        var wrongPassword = "WrongPassword123!";
        var hashedPassword = _passwordService.HashPassword(password);

        // Act
        var result = _passwordService.VerifyPassword(hashedPassword, wrongPassword);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void HashPassword_Should_Produce_Different_Hashes_For_Same_Password()
    {
        // Arrange
        var password = "TestPassword123!";

        // Act
        var hashedPassword1 = _passwordService.HashPassword(password);
        var hashedPassword2 = _passwordService.HashPassword(password);

        // Assert
        Assert.NotEqual(hashedPassword1, hashedPassword2);
    }
}
