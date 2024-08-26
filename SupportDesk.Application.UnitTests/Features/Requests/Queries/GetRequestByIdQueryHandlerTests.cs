using AutoMapper;
using Moq;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Features.Requests.Queries.GetRequestById;
using SupportDesk.Domain.Entities;
using Xunit;
using Shouldly;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Contracts.Services;
using SupportDesk.Application.Models.Dtos;

public class GetRequestByIdQueryHandlerTests
{
    private readonly Mock<IRequestRepository> _mockRequestRepository;
    private readonly IMapper _mapper;
    private readonly GetRequestByIdQueryHandler _handler;

    public GetRequestByIdQueryHandlerTests()
    {
        _mockRequestRepository = new Mock<IRequestRepository>();

        var configuration = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Request, RequestDto>();
        });

        _mapper = configuration.CreateMapper();

        _handler = new GetRequestByIdQueryHandler(
            _mockRequestRepository.Object,
            _mapper);
    }

    [Fact]
    public async Task Handle_Should_Return_RequestDto_When_Request_Exists()
    {
        // Arrange
        var request = new Request { Id = 1, RequestTypeId = 1, ZoneId = 1 };
        _mockRequestRepository.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);

        var query = new GetRequestByIdQuery { Id = 1 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.Request.ShouldNotBeNull();
        result.Request.Id.ShouldBe(1);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_Request_Not_Found()
    {
        // Arrange
        _mockRequestRepository.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Request)null);

        var query = new GetRequestByIdQuery { Id = 1 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.Message.ShouldBe(RequestMessages.RequestNotFound);
    }

    [Fact]
    public async Task Handle_Should_Return_Error_When_Request_Is_Inactive()
    {
        // Arrange
        var request = new Request { Id = 1, IsActive = false };
        _mockRequestRepository.Setup(repo => repo.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(request);


        var query = new GetRequestByIdQuery { Id = 1 };

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        result.ShouldNotBeNull();
        result.Success.ShouldBeFalse();
        result.Message.ShouldBe(RequestMessages.RequestIsInactive);
    }
}
