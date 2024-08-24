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

        [Fact]
        public async Task GetPendingRequestsAsync_Should_Return_Correct_Requests()
        {
            // Arrange
            var pendingThreshold = TimeSpan.FromHours(12);
            var now = DateTime.UtcNow;

            // Add test data to the in-memory database
            var requests = new List<Request>
            {
                new Request { Id = 40, CreatedDate = now.AddHours(-13), ReviewerUserId = null, IsActive = true },
                new Request { Id = 41, CreatedDate = now.AddHours(-14), ReviewerUserId = null, IsActive = true },
                new Request { Id = 42, CreatedDate = now.AddHours(-11), ReviewerUserId = null, IsActive = true }, // Should not be included
                new Request { Id = 43, CreatedDate = now.AddHours(-15), ReviewerUserId = Guid.NewGuid(), IsActive = true }, // Should not be included
                new Request { Id = 44, CreatedDate = now.AddHours(-16), ReviewerUserId = null, IsActive = false } // Should not be included
            };
            _dbContext.Requests.AddRange(requests);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetPendingRequestsAsync(pendingThreshold);

            // Assert
            Assert.NotNull(result);
            Assert.Contains(result, r => r.Id == 40);
            Assert.Contains(result, r => r.Id == 41);
        }

        [Fact]
        public async Task GetExpiringRequestsAsync_Should_Return_Expiring_Requests()
        {
            // Arrange
            var now = DateTime.UtcNow;
            var requests = new List<Request>
            {
                new Request { Id = 20, CreatedDate = now.AddHours(-21), RequestStatusId = 1, IsActive = true },
                new Request { Id = 21, CreatedDate = now.AddHours(-22), RequestStatusId = 1, IsActive = true },
                new Request { Id = 22, CreatedDate = now.AddHours(-5), RequestStatusId = 1, IsActive = true } // Not expiring
            };

            await _dbContext.Requests.AddRangeAsync(requests);
            await _dbContext.SaveChangesAsync();

            var expiringThreshold = TimeSpan.FromHours(20);

            // Act
            var expiringRequests = await _repository.GetExpiringRequestsAsync(expiringThreshold);

            // Assert
            Assert.NotNull(expiringRequests);
            Assert.DoesNotContain(expiringRequests, r => r.Id == 22);
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
                    ReviewerUserId = Guid.NewGuid(),
                    RequestStatusId = (int)RequestStatusesEnum.Approved,
                    IsActive = true
                },
                new Request
                {
                    CreatedDate = DateTime.UtcNow.AddHours(-5),
                    LastModifiedDate = DateTime.UtcNow.AddHours(-1),
                    ReviewerUserId = Guid.NewGuid(),
                    RequestStatusId = (int)RequestStatusesEnum.Approved,
                    IsActive = true
                }
            });
            await _dbContext.SaveChangesAsync();

            // Act
            var averageResponseTime = await _repository.GetAverageResponseTimeAsync(CancellationToken.None);

            // Assert
            // En este caso: (3 horas + 4 horas) / 2 = 3.5 horas
            var expectedTimeSpan = TimeSpan.FromHours(3.5);
            var tolerance = TimeSpan.FromMilliseconds(1);  // Tolerancia de 1 milisegundo

            Assert.InRange(averageResponseTime.TotalMilliseconds,
                           expectedTimeSpan.TotalMilliseconds - tolerance.TotalMilliseconds,
                           expectedTimeSpan.TotalMilliseconds + tolerance.TotalMilliseconds);
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
    }
}
