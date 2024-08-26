using AutoMapper;
using Moq;
using Shouldly;
using SupportDesk.Application.Contracts.Persistence;
using SupportDesk.Application.Features.Requests.Queries.GetMyRequests;
using SupportDesk.Application.Features.Users.Requests.Queries.GetMyRequests;
using SupportDesk.Application.Models;
using SupportDesk.Application.Profiles;
using SupportDesk.Domain.Entities;
using Xunit;

namespace SupportDesk.Application.UnitTests.Features.Users.Requests.Queries
{
    public class GetMyRequestsQueryHandlerTests
    {
        private readonly Mock<IRequestRepository> _requestRepositoryMock;
        private readonly IMapper _mapper;

        public GetMyRequestsQueryHandlerTests()
        {
            _requestRepositoryMock = new Mock<IRequestRepository>();

            var configurationProvider = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            });

            _mapper = configurationProvider.CreateMapper();
        }

        [Fact]
        public async Task Handle_Should_Return_Paged_Response_With_Requests()
        {
            // Arrange
            var query = new GetMyRequestsQuery
            {
                UserId = Guid.NewGuid(),
                Page = 1,
                PageSize = 10
            };

            var requests = new List<Request>
            {
                new Request { Id = 1, Comments = "Test request 1" },
                new Request { Id = 2, Comments = "Test request 2" }
            };

            _requestRepositoryMock
                .Setup(repo => repo.GetUserRequestsAsync(
                    query.UserId!.Value,
                    query.RequestTypeId,
                    query.StatusId,
                    query.CreatedFrom,
                    query.CreatedTo,
                    query.Page ?? PagedQuery.DefaultPage,
                    query.PageSize ?? PagedQuery.DefaultPageSize,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync((requests, requests.Count));

            var handler = new GetMyRequestsQueryHandler(_requestRepositoryMock.Object, _mapper);

            // Act
            var result = await handler.Handle(query, CancellationToken.None);

            // Assert
            result.ShouldNotBeNull();
            result.Requests.Count.ShouldBe(requests.Count);
            result.TotalCount.ShouldBe(requests.Count);
        }
    }
}
