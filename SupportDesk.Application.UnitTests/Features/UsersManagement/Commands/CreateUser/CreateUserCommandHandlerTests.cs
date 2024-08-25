using Moq;
using Xunit;
using SupportDesk.Application.Contracts.Persistence;
using AutoMapper;
using SupportDesk.Domain.Entities;
using SupportDesk.Application.Contracts.Infraestructure.FileStorage;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Features.UsersManagement.Commands.CreateUser;
using SupportDesk.Application.Models.Dtos;
using SupportDesk.Application.Contracts.Services;

namespace SupportDesk.Application.UnitTests.Features.Users.Commands.CreateUser;

public class CreateUserCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly Mock<ILogger<CreateUserCommandHandler>> _loggerMock;
    private readonly CreateUserCommandHandler _handler;

    public CreateUserCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _passwordServiceMock = new Mock<IPasswordService>();
        _mapperMock = new Mock<IMapper>();
        _loggerMock = new Mock<ILogger<CreateUserCommandHandler>>();

        _handler = new CreateUserCommandHandler(
            _userRepositoryMock.Object,
            _fileStorageServiceMock.Object,
            _passwordServiceMock.Object,
            _mapperMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Create_User_And_Return_Response()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "testuser@example.com",
            FirstName = "Test",
            LastName = "User",
            Password = "ValidPassword123",
            IsAdmin = false,
            IsSupervisor = false
        };

        var hashedPassword = "hashedPassword";
        _passwordServiceMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns(hashedPassword);

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            PasswordHash = hashedPassword,
            IsAdmin = command.IsAdmin,
            IsSupervisor = command.IsSupervisor,
            IsActive = true
        };

        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Id = user.Id });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.UserCreated);
        Assert.Equal(user.Id, result.UserCreated.Id);
        _userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Return_ValidationErrors_For_Invalid_Data()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "invalid-email", // Correo electrónico inválido
            FirstName = "", // Nombre vacío
            LastName = "", // Apellido vacío
            Password = "short" // Contraseña demasiado corta
        };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.NotNull(result.ValidationErrors);
        Assert.Contains(result.ValidationErrors, e => e.Contains(UsersMessages.InvalidEmail));
        Assert.Contains(result.ValidationErrors, e => e.Contains(UsersMessages.InvalidFirstName));
        Assert.Contains(result.ValidationErrors, e => e.Contains(UsersMessages.InvalidLastName));
        Assert.Contains(result.ValidationErrors, e => e.Contains(UsersMessages.InvalidPassword));
    }

    [Fact]
    public async Task Handle_Should_Call_FileStorageService_And_Save_User_With_Photo()
    {
        // Arrange
        var command = new CreateUserCommand
        {
            Email = "testuser@example.com",
            FirstName = "Test",
            LastName = "User",
            Password = "ValidPassword123",
            IsAdmin = false,
            IsSupervisor = false,
            Photo = MockFormFile("photo.jpg")
        };

        var hashedPassword = "hashedPassword";
        _passwordServiceMock.Setup(h => h.HashPassword(It.IsAny<string>())).Returns(hashedPassword);

        _fileStorageServiceMock.Setup(f => f.UploadFilesAsync(It.IsAny<List<IFormFile>>(), "users", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "http://localhost/files/users/photo.jpg" });

        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = command.Email,
            FirstName = command.FirstName,
            LastName = command.LastName,
            PasswordHash = hashedPassword,
            IsAdmin = command.IsAdmin,
            IsSupervisor = command.IsSupervisor,
            PhotoUrl = "http://localhost/files/users/photo.jpg",
            IsActive = true
        };

        _userRepositoryMock.Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(user);
        _mapperMock.Setup(m => m.Map<UserDto>(It.IsAny<User>())).Returns(new UserDto { Id = user.Id });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        _fileStorageServiceMock.Verify(x => x.UploadFilesAsync(It.Is<List<IFormFile>>(files => files.Count == 1), "users", It.IsAny<CancellationToken>()), Times.Once);
        Assert.True(result.Success);
        Assert.NotNull(result.UserCreated);
        Assert.Equal("http://localhost/files/users/photo.jpg", result.UserCreated.PhotoUrl);
    }

    private static IFormFile MockFormFile(string fileName)
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(100);

        return fileMock.Object;
    }
}
