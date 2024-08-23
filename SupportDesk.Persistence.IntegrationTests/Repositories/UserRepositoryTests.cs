using Microsoft.EntityFrameworkCore;
using Shouldly;
using SupportDesk.Domain.Entities;
using SupportDesk.Persistence.SupportDesk;
using SupportDesk.Persistence.SupportDesk.Repositories;
using Xunit;

namespace SupportDesk.Persistence.IntegrationTests.Repositories
{
    public class UserRepositoryTests
    {
        private readonly UserRepository _repository;
        private readonly ApplicationDbContext _dbContext;

        public UserRepositoryTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "SupportDeskTestDb")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _repository = new UserRepository(_dbContext);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), Email = "user1@test.com", FirstName = "John", LastName = "Doe" },
                new User { Id = Guid.NewGuid(), Email = "user2@test.com", FirstName = "Jane", LastName = "Smith" },
                new User { Id = Guid.NewGuid(), Email = "user3@test.com", FirstName = "Jim", LastName = "Brown" }
            };

            _dbContext.Users.AddRange(users);
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task GetUsersByIdsAsync_Should_Return_Users_When_Ids_Exist()
        {
            // Arrange
            var existingUserIds = _dbContext.Users.Select(u => u.Id).Take(2).ToList();

            // Act
            var result = await _repository.GetUsersByIdsAsync(existingUserIds);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2);
            result.ShouldAllBe(user => existingUserIds.Contains(user.Id));
        }

        [Fact]
        public async Task GetUsersByIdsAsync_Should_Return_Empty_List_When_Ids_Do_Not_Exist()
        {
            // Arrange
            var nonExistentIds = new List<Guid> { Guid.NewGuid(), Guid.NewGuid() };

            // Act
            var result = await _repository.GetUsersByIdsAsync(nonExistentIds);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(0);
        }
        
    }
}
