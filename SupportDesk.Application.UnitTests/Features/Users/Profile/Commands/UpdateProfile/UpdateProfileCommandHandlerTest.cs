using AutoMapper;
using Moq;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Features.Users.Profile.Commands.UpdateProfile;
using SupportDesk.Domain.Entities;
using Xunit;
using Shouldly;
using SupportDesk.Application.Contracts.Infraestructure.FileStorage;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using SupportDesk.Application.Models.Dtos;
using SupportDesk.Application.Constants;

namespace SupportDesk.Application.UnitTests.Features.Users.Profile.Commands.UpdateProfile;

public class UpdateProfileCommandHandlerTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly IMapper _mapper;
    private readonly Mock<ILogger<UpdateProfileCommandHandler>> _loggerMock;
    private readonly UpdateProfileCommandHandler _handler;

    public UpdateProfileCommandHandlerTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _fileStorageServiceMock = new Mock<IFileStorageService>();
        _loggerMock = new Mock<ILogger<UpdateProfileCommandHandler>>();

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<User, UserDto>();
        });

        _mapper = configuration.CreateMapper();

        _handler = new UpdateProfileCommandHandler(
            _userRepositoryMock.Object,
            _fileStorageServiceMock.Object,
            _mapper,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Handle_Should_Update_User_And_Return_Response()
    {
        // Arrange
        var command = new UpdateProfileCommand
        {
            UserId = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
            GenderId = 1
        };

        var user = new User
        {
            Id = command.UserId,
            FirstName = "OldFirstName",
            LastName = "OldLastName",
            BirthDate = new DateTime(1980, 1, 1),
            GenderId = 1,
            IsActive = true
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.UserUpdated.ShouldNotBeNull();
        result.UserUpdated.FirstName.ShouldBe(command.FirstName);
        result.UserUpdated.LastName.ShouldBe(command.LastName);
        result.UserUpdated.BirthDate.ShouldBe(command.DateOfBirth);
        result.UserUpdated.GenderId.ShouldBe(command.GenderId);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_User_Not_Found()
    {
        // Arrange
        var command = new UpdateProfileCommand { UserId = Guid.NewGuid() };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_User_Is_Inactive()
    {
        // Arrange
        var command = new UpdateProfileCommand { UserId = Guid.NewGuid() };

        var user = new User { Id = command.UserId, IsActive = false };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
    }

    [Fact]
    public async Task Handle_Should_Update_User_And_Save_Photo()
    {
        // Arrange
        var command = new UpdateProfileCommand
        {
            UserId = Guid.NewGuid(),
            FirstName = "Test",
            LastName = "User",
            DateOfBirth = new DateTime(1990, 1, 1),
            GenderId = 1,
            Photo = MockFormFile("newphoto.jpg")
        };

        var user = new User
        {
            Id = command.UserId,
            FirstName = "OldFirstName",
            LastName = "OldLastName",
            BirthDate = new DateTime(1980, 1, 1),
            GenderId = 1,
            IsActive = true
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _fileStorageServiceMock.Setup(f => f.UploadFilesAsync(It.IsAny<List<IFormFile>>(), "users", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<string> { "http://localhost/files/users/newphoto.jpg" });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.UserUpdated.PhotoUrl.ShouldBe("http://localhost/files/users/newphoto.jpg");
    }

    private static IFormFile MockFormFile(string fileName)
    {
        var fileMock = new Mock<IFormFile>();
        fileMock.Setup(f => f.FileName).Returns(fileName);
        fileMock.Setup(f => f.Length).Returns(100);

        return fileMock.Object;
    }
}
