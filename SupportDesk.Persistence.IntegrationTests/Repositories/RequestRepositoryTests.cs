using Microsoft.EntityFrameworkCore;
using SupportDesk.Domain.Entities;
using SupportDesk.Domain.Enums;
using SupportDesk.Persistence.SupportDesk.Repositories;
using SupportDesk.Persistence.SupportDesk;
using Xunit;

namespace SupportDesk.Persistence.IntegrationTests.Repositories
{
    public class RequestRepositoryTests
    {
        private readonly RequestRepository _repository;
        private readonly ApplicationDbContext _dbContext;

        public RequestRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "SupportDeskDb")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _repository = new RequestRepository(_dbContext);
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
    }

}
