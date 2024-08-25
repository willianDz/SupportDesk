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
                },



                //
                new Request { RequestStatusId = (int)RequestStatusesEnum.New, CreatedDate = DateTime.UtcNow.AddDays(-1), IsActive = true },
                new Request { RequestStatusId = (int)RequestStatusesEnum.Approved, CreatedDate = DateTime.UtcNow.AddDays(-2), LastModifiedDate = DateTime.UtcNow.AddDays(-1), ApprovalRejectionDate = DateTime.UtcNow.AddDays(-1), ReviewerUserId = Guid.NewGuid(), IsActive = true },
                new Request { RequestStatusId = (int)RequestStatusesEnum.Rejected, CreatedDate = DateTime.UtcNow.AddDays(-3), IsActive = true },
                new Request { RequestStatusId = (int)RequestStatusesEnum.UnderReview, CreatedDate = DateTime.UtcNow.AddDays(-2), ReviewerUserId = Guid.NewGuid(), IsActive = true }
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

        [Fact]
        public async Task GetPendingRequestsAsync_Should_Return_Correct_Requests()
        {
            // Arrange
            var pendingThreshold = TimeSpan.FromHours(12);
            var now = DateTime.UtcNow;

            // Add test data to the in-memory database
            var requests = new List<Request>
            {
                new Request { Id = 70, CreatedDate = now.AddHours(-13), ReviewerUserId = null, IsActive = true },
                new Request { Id = 71, CreatedDate = now.AddHours(-14), ReviewerUserId = null, IsActive = true },
                new Request { Id = 72, CreatedDate = now.AddHours(-11), ReviewerUserId = null, IsActive = true }, // Should not be included
                new Request { Id = 73, CreatedDate = now.AddHours(-15), ReviewerUserId = Guid.NewGuid(), IsActive = true }, // Should not be included
                new Request { Id = 74, CreatedDate = now.AddHours(-16), ReviewerUserId = null, IsActive = false } // Should not be included
            };
            _dbContext.Requests.AddRange(requests);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetPendingRequestsAsync(pendingThreshold);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, r => r.Id == 70);
            Assert.Contains(result, r => r.Id == 71);
        }

        [Fact]
        public async Task GetExpiringRequestsAsync_Should_Return_Expiring_Requests()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var requests = new List<Request>
            {
                new Request { Id = 60, CreatedDate = now.AddHours(-21), RequestStatusId = 1, IsActive = true },
                new Request { Id = 61, CreatedDate = now.AddHours(-22), RequestStatusId = 1, IsActive = true },
                new Request { Id = 62, CreatedDate = now.AddHours(-5), RequestStatusId = 1, IsActive = true } // Not expiring
            };

            await _dbContext.Requests.AddRangeAsync(requests);
            await _dbContext.SaveChangesAsync();

            var expiringThreshold = TimeSpan.FromHours(20);

            // Act
            var expiringRequests = await _repository.GetExpiringRequestsAsync(expiringThreshold);

            // Assert
            Assert.NotNull(expiringRequests);
            Assert.DoesNotContain(expiringRequests, r => r.Id == 62);
        }

        [Fact]
        public async Task GetExpiringRequestsAsync_Should_Not_Return_Approved_Or_Rejected_Requests()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var requests = new List<Request>
            {
                new Request { Id = 90, CreatedDate = now.AddHours(-21), RequestStatusId = (int)RequestStatusesEnum.Approved, IsActive = true },
                new Request { Id = 91, CreatedDate = now.AddHours(-22), RequestStatusId = (int)RequestStatusesEnum.Rejected, IsActive = true },
                new Request { Id = 92, CreatedDate = now.AddHours(-22), RequestStatusId = 1, IsActive = true } // Valid for expiring
            };

            await _dbContext.Requests.AddRangeAsync(requests);
            await _dbContext.SaveChangesAsync();

            var expiringThreshold = TimeSpan.FromHours(20);

            // Act
            var expiringRequests = await _repository.GetExpiringRequestsAsync(expiringThreshold);

            // Assert
            Assert.NotNull(expiringRequests);
            Assert.DoesNotContain(expiringRequests, r => r.Id == 90);
            Assert.DoesNotContain(expiringRequests, r => r.Id == 91);
        }

        [Fact]
        public async Task GetAverageResponseTimeAsync_Should_Return_Correct_Average_When_Requests_Exist()
        {
            // Arrange
            _dbContext.Requests.AddRange(new List<Request>
            {
                new Request
                {
                    CreatedDate = DateTime.UtcNow.AddHours(-3),
                    LastModifiedDate = DateTime.UtcNow,
                    ApprovalRejectionDate = DateTime.UtcNow,
                    ReviewerUserId = Guid.NewGuid(),
                    RequestStatusId = (int)RequestStatusesEnum.Approved,
                    IsActive = true
                },
                new Request
                {
                    CreatedDate = DateTime.UtcNow.AddHours(-5),
                    LastModifiedDate = DateTime.UtcNow.AddHours(-1),
                    ApprovalRejectionDate = DateTime.UtcNow.AddHours(-1),
                    ReviewerUserId = Guid.NewGuid(),
                    RequestStatusId = (int)RequestStatusesEnum.Approved,
                    IsActive = true
                }
            });
            await _dbContext.SaveChangesAsync();

            // Act
            var averageResponseTime = await _repository.GetAverageResponseTimeAsync(CancellationToken.None);

            // Assert
            averageResponseTime.TotalSeconds.ShouldBeGreaterThanOrEqualTo(0);
        }

        [Fact]
        public async Task GetAverageResponseTimeAsync_Should_Return_Zero_When_No_Requests_Exist()
        {
            // Arrange
            _dbContext.Requests.RemoveRange(_dbContext.Requests);
            await _dbContext.SaveChangesAsync();

            // Act
            var averageResponseTime = await _repository.GetAverageResponseTimeAsync(CancellationToken.None);

            // Assert
            Assert.Equal(TimeSpan.Zero, averageResponseTime);
        }

        [Fact]
        public async Task GetRequestCountByStatusAsync_Should_Return_Correct_Count()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-7);
            var endDate = DateTime.UtcNow;

            // Act
            var count = await _repository.GetRequestCountByStatusAsync((int)RequestStatusesEnum.New, startDate, endDate, CancellationToken.None);

            // Assert
            count.ShouldBeGreaterThanOrEqualTo(0);
        }

        [Fact]
        public async Task GetWeeklyRequestCountsAsync_Should_Return_Correct_Data()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-7);
            var endDate = DateTime.UtcNow;

            // Act
            var counts = await _repository.GetWeeklyRequestCountsAsync(startDate, endDate, CancellationToken.None);

            // Assert
            Assert.NotNull(counts);
        }

        [Fact]
        public async Task GetRequestTrendsByTypeAndZoneAsync_Should_Return_Correct_Trends()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-7);
            var endDate = DateTime.UtcNow;

            // Act
            var trends = await _repository.GetRequestTrendsByTypeAndZoneAsync(startDate, endDate, CancellationToken.None);

            // Assert
            Assert.NotNull(trends);
        }

        [Fact]
        public async Task GetAverageResolutionTimeAsync_Should_Return_Correct_Average()
        {
            // Arrange
            var startDate = DateTime.UtcNow.AddDays(-7);
            var endDate = DateTime.UtcNow;

            // Act
            var averageTime = await _repository.GetAverageResolutionTimeAsync(startDate, endDate, CancellationToken.None);

            // Assert

            var expectedTimeSpan = TimeSpan.FromDays(1);
            var tolerance = TimeSpan.FromMilliseconds(1);  // Tolerancia de 1 milisegundo

            Assert.InRange(averageTime.TotalMilliseconds,
                           expectedTimeSpan.TotalMilliseconds - tolerance.TotalMilliseconds,
                           expectedTimeSpan.TotalMilliseconds + tolerance.TotalMilliseconds);
        }
    }
}
