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
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "admin@example.com",
                    FirstName = "Admin",
                    LastName = "User",
                    IsAdmin = true,
                    IsSupervisor = false,
                    UserRequestTypes = new List<UserRequestType>
                    {
                        new UserRequestType { RequestTypeId = 1 }
                    },
                    UserZones = new List<UserZone>
                    {
                        new UserZone { ZoneId = 1 }
                    }
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "supervisor@example.com",
                    FirstName = "Supervisor",
                    LastName = "User",
                    IsAdmin = false,
                    IsSupervisor = true,
                    UserRequestTypes = new List<UserRequestType>
                    {
                        new UserRequestType { RequestTypeId = 1 }
                    },
                    UserZones = new List<UserZone>
                    {
                        new UserZone { ZoneId = 1 }
                    }
                },
                new User
                {
                    Id = Guid.NewGuid(),
                    Email = "user@example.com",
                    FirstName = "Regular",
                    LastName = "User",
                    IsAdmin = false,
                    IsSupervisor = false,
                    UserRequestTypes = new List<UserRequestType>
                    {
                        new UserRequestType { RequestTypeId = 2 }
                    },
                    UserZones = new List<UserZone>
                    {
                        new UserZone { ZoneId = 2 }
                    }
                }
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

        [Fact]
        public async Task GetSupervisorsAndAdminsForRequestAsync_Should_Return_Supervisors_And_Admins()
        {
            // Act
            var result = await _repository.GetSupervisorsAndAdminsForRequestAsync(1, 1);

            // Assert
            result.ShouldNotBeNull();
            result.Count.ShouldBe(2); // Only admin and supervisor should match
            result.ShouldContain(u => u.Email == "admin@example.com");
            result.ShouldContain(u => u.Email == "supervisor@example.com");
        }

        [Fact]
        public async Task GetSupervisorsAndAdminsForRequestAsync_Should_Return_Empty_List_If_No_Match()
        {
            // Act
            var result = await _repository.GetSupervisorsAndAdminsForRequestAsync(3, 1);

            // Assert
            result.ShouldBeEmpty(); // No users should match for request type 3
        }
    }
}
