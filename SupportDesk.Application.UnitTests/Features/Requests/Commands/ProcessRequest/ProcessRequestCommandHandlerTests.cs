﻿using AutoMapper;
using Moq;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Contracts.Services;
using SupportDesk.Application.Features.Requests.Commands.ProcessRequest;
using SupportDesk.Application.Models.Dtos;
using SupportDesk.Application.Services;
using SupportDesk.Application.Constants;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;
using Xunit;

namespace SupportDesk.Application.UnitTests.Features.Requests.Commands.ProcessRequest;

public class ProcessRequestCommandHandlerTests
{
    private readonly Mock<IRequestRepository> _mockRequestRepository;
    private readonly Mock<IRequestValidationService> _mockValidationService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly ProcessRequestCommandHandler _handler;

    public ProcessRequestCommandHandlerTests()
    {
        _mockRequestRepository = new Mock<IRequestRepository>();
        _mockValidationService = new Mock<IRequestValidationService>();
        _mockMapper = new Mock<IMapper>();

        _handler = new ProcessRequestCommandHandler(
            _mockRequestRepository.Object,
            _mockValidationService.Object,
            _mockMapper.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_Request_Not_Found()
    {
        // Arrange
        _mockRequestRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Request?)null);

        var command = new ProcessRequestCommand { RequestId = 1, UserId = Guid.NewGuid(), NewStatusId = (int)RequestStatusesEnum.Approved };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(RequestMessages.RequestNotFound, result.Message);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_Validation_Fails()
    {
        // Arrange
        var request = new Request { Id = 1, RequestStatusId = (int)RequestStatusesEnum.New };

        _mockRequestRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        _mockValidationService.Setup(v => v.ValidateUserCanProcessRequestAsync(It.IsAny<Request>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException(RequestMessages.InactiveRequestCannotBeProcessed));

        var command = new ProcessRequestCommand { RequestId = 1, UserId = Guid.NewGuid(), NewStatusId = (int)RequestStatusesEnum.Approved };

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.False(result.Success);
        Assert.Equal(RequestMessages.InactiveRequestCannotBeProcessed, result.Message);
    }

    [Fact]
    public async Task Handle_Should_Update_Request_Status_To_InReview()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new Request { Id = 1, RequestStatusId = (int)RequestStatusesEnum.New };

        _mockRequestRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        _mockValidationService.Setup(v => v.ValidateUserCanProcessRequestAsync(It.IsAny<Request>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new ProcessRequestCommand { RequestId = 1, UserId = userId, NewStatusId = (int)RequestStatusesEnum.UnderReview };

        _mockMapper.Setup(m => m.Map<RequestDto>(It.IsAny<Request>()))
            .Returns(new RequestDto { Id = request.Id, RequestStatusId = command.NewStatusId });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.ProcessedRequest);
        Assert.Equal((int)RequestStatusesEnum.UnderReview, result.ProcessedRequest.RequestStatusId);

        _mockRequestRepository.Verify(r => r.UpdateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.Equal(userId, request.ReviewerUserId);
        Assert.Equal(command.NewStatusId, request.RequestStatusId);
    }

    [Fact]
    public async Task Handle_Should_Update_Request_Status_To_Approved()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new Request { Id = 1, RequestStatusId = (int)RequestStatusesEnum.UnderReview };

        _mockRequestRepository.Setup(r => r.GetByIdAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        _mockValidationService.Setup(v => v.ValidateUserCanProcessRequestAsync(It.IsAny<Request>(), It.IsAny<Guid>(), It.IsAny<int>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        var command = new ProcessRequestCommand { RequestId = 1, UserId = userId, NewStatusId = (int)RequestStatusesEnum.Approved };

        _mockMapper.Setup(m => m.Map<RequestDto>(It.IsAny<Request>()))
            .Returns(new RequestDto { Id = request.Id, RequestStatusId = command.NewStatusId });

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.True(result.Success);
        Assert.NotNull(result.ProcessedRequest);
        Assert.Equal((int)RequestStatusesEnum.Approved, result.ProcessedRequest.RequestStatusId);

        _mockRequestRepository.Verify(r => r.UpdateAsync(It.IsAny<Request>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}