using Moq;
using Xunit;
using SupportDesk.Application.Contracts.Persistence;
using AutoMapper;
using SupportDesk.Application.Features.UsersManagement.Commands.InactivateUser;
using SupportDesk.Domain.Entities;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Models.Dtos;
using SupportDesk.Application.Constants;

namespace SupportDesk.Application.UnitTests.Features.Users.Commands.InactivateUser;

public class InactivateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<InactivateUserCommandHandler>> _loggerMock;
    private readonly InactivateUserCommandHandler _handler;

    public InactivateUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<InactivateUserCommandHandler>>();

        _handler = new InactivateUserCommandHandler(
            _userRepositoryMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Inactivate_User_And_Return_Response()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, IsActive = true };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Id = user.Id });

        // Act
        var result = await _handler.Handle(new InactivateUserCommand { UserId = userId }, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.InactivatedUser);
        Assert.Equal(user.Id, result.InactivatedUser?.Id);
        Assert.False(user.IsActive);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_User_Not_Found()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _handler.Handle(new InactivateUserCommand { UserId = Guid.NewGuid() }, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Null(result.InactivatedUser);
        Assert.Equal(UsersMessages.UserNotFound, result.Message);
    }
}
