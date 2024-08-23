using Microsoft.EntityFrameworkCore;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;
using SupportDesk.Persistence.SupportDesk.Repositories;
using SupportDesk.Persistence.SupportDesk;
using Xunit;
using Shouldly;

namespace SupportDesk.Persistence.IntegrationTests.Repositories
{
    public class RequestRepositoryTests
    {
        private readonly RequestRepository _repository;
        private readonly ApplicationDbContext _dbContext;
        private readonly Guid _testUserId;

        public RequestRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "SupportDeskDb")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _repository = new RequestRepository(_dbContext);
            _testUserId = Guid.NewGuid();

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            // Seed the database with test data
            _dbContext.Requests.AddRange(
                new Request
                {
                    RequestTypeId = 1,
                    ZoneId = 1,
                    Comments = "Valid Comment for Request 1",
                    RequestStatusId = 1,
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = _testUserId,
                },
                new Request
                {
                    RequestTypeId = 1,
                    ZoneId = 2,
                    Comments = "Valid Comment for Request 2",
                    RequestStatusId = 2,
                    CreatedDate = DateTime.UtcNow.AddDays(-1),
                    CreatedBy = _testUserId,
                },
                new Request
                {
                    RequestTypeId = 2,
                    ZoneId = 1,
                    Comments = "Valid Comment for Request 3",
                    RequestStatusId = 3,
                    CreatedDate = DateTime.UtcNow.AddDays(-2),
                    CreatedBy = _testUserId,
                }
            );

            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task AddAsync_Should_Add_Request_To_Database()
        {
            // Arrange
            var request = new Request
            {
                RequestTypeId = 1,
                ZoneId = 1,
                Comments = "Test comments",
                RequestStatusId = (int)RequestStatusesEnum.New
            };

            // Act
            await _repository.AddAsync(request);

            // Assert
            var addedRequest = await _dbContext.Requests.FindAsync(request.Id);
            Assert.NotNull(addedRequest);
            Assert.Equal(request.RequestTypeId, addedRequest.RequestTypeId);
            Assert.Equal(request.ZoneId, addedRequest.ZoneId);
            Assert.Equal(request.Comments, addedRequest.Comments);
        }

        [Fact]
        public async Task GetUserRequestsAsync_Should_Return_Requests_With_Pagination_And_Filters()
        {
            // Arrange
            var userId = _testUserId;
            var requestTypeId = 1;
            var statusId = 1;
            var createdFrom = DateTime.UtcNow.AddDays(-3);
            var createdTo = DateTime.UtcNow.AddDays(1);
            int page = 1;
            int pageSize = 10;

            // Act
            var (requests, totalCount) = await _repository.GetUserRequestsAsync(
                userId,
                requestTypeId,
                statusId,
                createdFrom,
                createdTo,
                page,
                pageSize);

            // Assert
            requests.ShouldNotBeEmpty();
            requests.Count.ShouldBe(1);
            totalCount.ShouldBe(1);
            requests[0].Comments.ShouldBe("Valid Comment for Request 1");
        }

        [Fact]
        public async Task GetUserRequestsAsync_Should_Return_Empty_If_No_Match()
        {
            // Arrange
            var userId = _testUserId;
            var requestTypeId = 99; // non-existent request type
            var statusId = 99; // non-existent status
            var createdFrom = DateTime.UtcNow.AddDays(-3);
            var createdTo = DateTime.UtcNow.AddDays(1);
            int page = 1;
            int pageSize = 10;

            // Act
            var (requests, totalCount) = await _repository.GetUserRequestsAsync(
                userId,
                requestTypeId,
                statusId,
                createdFrom,
                createdTo,
                page,
                pageSize);

            // Assert
            requests.ShouldBeEmpty();
            totalCount.ShouldBe(0);
        }
    }
}
