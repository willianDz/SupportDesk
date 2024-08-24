using Microsoft.EntityFrameworkCore;
using Shouldly;
using SupportDesk.Application.Contracts.Persistence;
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
            result.Count.ShouldBeGreaterThanOrEqualTo(2);
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

        [Fact]
        public async Task GetSupervisorsByZoneAndRequestTypeAsync_Should_Return_Correct_Supervisors()
        {
            // Arrange
            var zoneId = 1;
            var requestTypeId = 1;

            var users = new List<User>
            {
                new User { Id = Guid.NewGuid(), IsSupervisor = true, IsActive = true, UserZones = new List<UserZone>{ new UserZone{ ZoneId = zoneId } }, UserRequestTypes = new List<UserRequestType>{ new UserRequestType{ RequestTypeId = requestTypeId } } },
                new User { Id = Guid.NewGuid(), IsSupervisor = true, IsActive = true, UserZones = new List<UserZone>{ new UserZone{ ZoneId = 2 } }, UserRequestTypes = new List<UserRequestType>{ new UserRequestType{ RequestTypeId = requestTypeId } } }, // Different zone
                new User { Id = Guid.NewGuid(), IsSupervisor = true, IsActive = false, UserZones = new List<UserZone>{ new UserZone{ ZoneId = zoneId } }, UserRequestTypes = new List<UserRequestType>{ new UserRequestType{ RequestTypeId = requestTypeId } } }, // Inactive
                new User { Id = Guid.NewGuid(), IsSupervisor = true, IsActive = true, UserZones = new List<UserZone>{ new UserZone{ ZoneId = zoneId } }, UserRequestTypes = new List<UserRequestType>{ new UserRequestType{ RequestTypeId = 2 } } } // Different request type
            };
            _dbContext.Users.AddRange(users);
            await _dbContext.SaveChangesAsync();

            // Act
            var result = await _repository.GetSupervisorsByZoneAndRequestTypeAsync(zoneId, requestTypeId);

            // Assert
            Assert.NotNull(result);
            result.Count.ShouldBeGreaterThanOrEqualTo(1);
            Assert.Equal(zoneId, result.First().UserZones.First().ZoneId);
            Assert.Equal(requestTypeId, result.First().UserRequestTypes.First().RequestTypeId);
        }

        [Fact]
        public async Task GetAdminUsersAsync_Should_Return_Admin_Users()
        {
            // Arrange
            var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), IsAdmin = true, IsActive = true },
            new User { Id = Guid.NewGuid(), IsAdmin = true, IsActive = true },
            new User { Id = Guid.NewGuid(), IsAdmin = false, IsActive = true } // Not an admin
        };

            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            // Act
            var adminUsers = await _repository.GetAdminUsersAsync();

            // Assert
            Assert.NotNull(adminUsers);
            adminUsers.Count.ShouldBeGreaterThanOrEqualTo(2);
            Assert.All(adminUsers, u => Assert.True(u.IsAdmin));
        }

        [Fact]
        public async Task GetAdminUsersAsync_Should_Not_Return_Inactive_Admin_Users()
        {
            // Arrange
            var users = new List<User>
        {
            new User { Id = Guid.NewGuid(), IsAdmin = true, IsActive = true },
            new User { Id = Guid.NewGuid(), IsAdmin = true, IsActive = false }, // Inactive admin
            new User { Id = Guid.NewGuid(), IsAdmin = true, IsActive = true }
        };

            await _dbContext.Users.AddRangeAsync(users);
            await _dbContext.SaveChangesAsync();

            // Act
            var adminUsers = await _repository.GetAdminUsersAsync();

            // Assert
            Assert.NotNull(adminUsers);
            adminUsers.Count.ShouldBeGreaterThanOrEqualTo(2);
            Assert.All(adminUsers, u => Assert.True(u.IsActive));
        }
    }
}
