using Moq;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Services;
using SupportDesk.Application.Constants;
using SupportDesk.Domain.Entities;
using Xunit;
using SupportDesk.Domain.Enums;

namespace SupportDesk.Application.UnitTests.Services;

public class RequestValidationServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly RequestValidationService _validationService;

    public RequestValidationServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _validationService = new RequestValidationService(_mockUserRepository.Object);
    }

    [Fact]
    public async Task ValidateUserCanProcessRequestAsync_Should_Throw_Exception_For_Inactive_Request()
    {
        // Arrange
        var request = new Request { Id = 1, IsActive = false };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _validationService.ValidateUserCanProcessRequestAsync(request, Guid.NewGuid(), (int)RequestStatusesEnum.UnderReview, string.Empty));

        Assert.Equal(RequestMessages.InactiveRequestCannotBeProcessed, ex.Message);
    }

    [Fact]
    public async Task ValidateUserCanProcessRequestAsync_Should_Throw_Exception_For_Already_Approved_Request()
    {
        // Arrange
        var request = new Request { Id = 1, IsActive = true, RequestStatusId = (int)RequestStatusesEnum.Approved };

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _validationService.ValidateUserCanProcessRequestAsync(request, Guid.NewGuid(), (int)RequestStatusesEnum.Rejected, string.Empty));

        Assert.Equal(RequestMessages.RequestAlreadyApprovedOrRejected, ex.Message);
    }

    [Fact]
    public async Task ValidateUserCanProcessRequestAsync_Should_Throw_Exception_When_User_Has_No_Zone_Permission()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new Request { Id = 1, IsActive = true, RequestStatusId = (int)RequestStatusesEnum.New, ZoneId = 1 };
        var user = new User { Id = userId, UserZones = new List<UserZone>() }; // No zones assigned to user

        _mockUserRepository.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _validationService.ValidateUserCanProcessRequestAsync(request, userId, (int)RequestStatusesEnum.UnderReview, string.Empty));

        Assert.Equal(RequestMessages.UserNoZonePermission, ex.Message);
    }

    [Fact]
    public async Task ValidateUserCanProcessRequestAsync_Should_Throw_Exception_When_User_Has_No_RequestType_Permission()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new Request { Id = 1, IsActive = true, RequestStatusId = (int)RequestStatusesEnum.New, RequestTypeId = 1 };
        var user = new User { Id = userId, UserRequestTypes = new List<UserRequestType>() }; // No request types assigned to user

        _mockUserRepository.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _validationService.ValidateUserCanProcessRequestAsync(request, userId, (int)RequestStatusesEnum.UnderReview, string.Empty));

        Assert.Equal(RequestMessages.UserNoRequestTypePermission, ex.Message);
    }

    [Fact]
    public async Task ValidateUserCanProcessRequestAsync_Should_Throw_Exception_When_Reviewer_User_Comments_Are_Too_Shorts()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new Request { Id = 1, IsActive = true, RequestStatusId = (int)RequestStatusesEnum.New, RequestTypeId = 1 };
        var user = new User { Id = userId, UserRequestTypes = new List<UserRequestType>() }; // No request types assigned to user

        _mockUserRepository.Setup(u => u.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        // Act & Assert
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _validationService.ValidateUserCanProcessRequestAsync(request, userId, (int)RequestStatusesEnum.Approved, "Short comment"));

        Assert.Equal(RequestMessages.UserNoRequestTypePermission, ex.Message);
    }
}