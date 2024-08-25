using Moq;
using Xunit;
using SupportDesk.Application.Contracts.Persistence;
using AutoMapper;
using SupportDesk.Domain.Entities;
using SupportDesk.Application.Contracts.Infraestructure.FileStorage;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Features.UsersManagement.Commands.UpdateUser;
using SupportDesk.Application.Contracts.Services;
using SupportDesk.Application.Models.Dtos;
using Microsoft.AspNetCore.Http;
using SupportDesk.Application.Constants;

namespace SupportDesk.Application.UnitTests.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<UpdateUserCommandHandler>> _loggerMock;
    private readonly UpdateUserCommandHandler _handler;

    public UpdateUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _passwordServiceMock = new Mock<IPasswordService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<UpdateUserCommandHandler>>();

        _handler = new UpdateUserCommandHandler(
            _userRepositoryMock.Object,
            _fileStorageServiceMock.Object,
            _passwordServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Update_User_And_Return_Response()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "testuser@example.com",
            FirstName = "Test",
            LastName = "User",
            IsAdmin = false,
            IsSupervisor = false,
            IsActive = true
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var command = new UpdateUserCommand
        {
            UserId = user.Id,
            Email = "updatedemail@example.com",
            FirstName = "Updated",
            LastName = "User",
            IsAdmin = true,
            IsSupervisor = true,
            IsActive = false
        };

        _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Id = user.Id });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.UpdatedUser);
        Assert.Equal(user.Id, result.UpdatedUser.Id);
        Assert.Equal("updatedemail@example.com", user.Email);
        Assert.Equal("Updated", user.FirstName);
        Assert.Equal("User", user.LastName);
        Assert.True(user.IsAdmin);
        Assert.True(user.IsSupervisor);
        Assert.False(user.IsActive);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_If_User_Not_Found()
    {
        // Arrange
        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        var command = new UpdateUserCommand
        {
            UserId = Guid.NewGuid(),
            Email = "updatedemail@example.com",
            FirstName = "Updated",
            LastName = "User",
            IsAdmin = true,
            IsSupervisor = true,
            IsActive = false
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(UsersMessages.UserNotFound, result.Message);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Update_User_And_Save_Photo()
    {
        // Arrange
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = "testuser@example.com",
            FirstName = "Test",
            LastName = "User",
            IsAdmin = false,
            IsSupervisor = false,
            IsActive = true,
            PhotoUrl = "http://localhost/oldphoto.jpg"
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _userRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _fileStorageServiceMock.Setup(f => f.UploadFilesAsync(It.IsAny<List<IFormFile>>(), "users", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "http://localhost/files/users/photo.jpg" });

        var command = new UpdateUserCommand
        {
            UserId = user.Id,
            Email = "updatedemail@example.com",
            FirstName = "Updated",
            LastName = "User",
            IsAdmin = true,
            IsSupervisor = true,
            IsActive = false,
            Photo = MockFormFile("photo.jpg")
        };

        _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Id = user.Id });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.UpdatedUser);
        Assert.Equal(user.Id, result.UpdatedUser.Id);
        Assert.Equal("http://localhost/files/users/photo.jpg", user.PhotoUrl);
        _fileStorageServiceMock.Verify(x => x.UploadFilesAsync(It.Is<List<IFormFile>>(files => files.Count == 1), "users", It.IsAny<CancellationToken>()), Times.Once);
        _userRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    private static IFormFile MockFormFile(string fileName)
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(100);

        return fileMock.Object;
    }
}
