using AutoMapper;
using Moq;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Domain.Entities;
using Xunit;
using Shouldly;
using SupportDesk.Application.Features.Users.Summary;

namespace SupportDesk.Application.UnitTests.Features.Users.Summary;

public class GetUserSummaryQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRequestRepository> _mockRequestRepository;
    private readonly GetUserSummaryQueryHandler _handler;

    public GetUserSummaryQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRequestRepository = new Mock<IRequestRepository>();

        _handler = new GetUserSummaryQueryHandler(
            _mockRequestRepository.Object,
            _mockUserRepository.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Summary_When_User_Exists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, IsActive = true };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var query = new GetUserSummaryQuery { UserId = userId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_User_Not_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        var query = new GetUserSummaryQuery { UserId = userId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_User_Is_Inactive()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, IsActive = false };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var query = new GetUserSummaryQuery { UserId = userId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
    }
}
