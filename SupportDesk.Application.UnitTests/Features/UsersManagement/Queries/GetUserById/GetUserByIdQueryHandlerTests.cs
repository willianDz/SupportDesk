using AutoMapper;
using Moq;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Features.UsersManagement.Queries.GetUserById;
using SupportDesk.Domain.Entities;
using Xunit;
using Shouldly;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Models.Dtos;

namespace SupportDesk.Application.UnitTests.Features.UsersManagement.Queries.GetUserById;

public class GetUserByIdQueryHandlerTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly IMapper _mapper;
    private readonly GetUserByIdQueryHandler _handler;

    public GetUserByIdQueryHandlerTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserDto>();
        });

        _mapper = configuration.CreateMapper();

        _handler = new GetUserByIdQueryHandler(_mockUserRepository.Object, _mapper);
    }

    [Fact]
    public async Task Handle_Should_Return_UserDto_When_User_Exists()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, FirstName = "Test", LastName = "User", IsActive = true };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var query = new GetUserByIdQuery { UserId = userId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.User.ShouldNotBeNull();
        result.User.Id.ShouldBe(userId);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_User_Not_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        var query = new GetUserByIdQuery { UserId = userId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.Message.ShouldBe(UsersMessages.UserNotFound);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_User_Is_Inactive()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User { Id = userId, IsActive = false };
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        var query = new GetUserByIdQuery { UserId = userId };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.Message.ShouldBe(UsersMessages.UserIsInactive);
    }
}
