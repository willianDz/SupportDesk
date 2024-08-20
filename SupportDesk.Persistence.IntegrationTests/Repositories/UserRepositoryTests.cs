using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Domain.Entities;
using SupportDesk.Persistence.IntegrationTests.Helpers;
using Xunit;

namespace SupportDesk.Persistence.IntegrationTests.Repositories
{
    

    public class UserRepositoryTests
    {
        [Fact]
        public async Task CanPerformCRUDOnUser()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            var user = new User
            {
                Id = Guid.NewGuid(),
                FirstName = "Juan",
                LastName = "Perez",
                BirthDate = new DateTime(1990, 1, 1),
                GenderId = 1,
                PhotoUrl = "https://example.com/photo.jpg"
            };

            // Create
            await context.Users.AddAsync(user);
            await context.SaveChangesAsync();

            // Read
            var fetchedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            fetchedUser.Should().NotBeNull();
            fetchedUser!.FirstName.Should().Be("Juan");

            // Update
            fetchedUser.LastName = "Lopez";
            context.Users.Update(fetchedUser);
            await context.SaveChangesAsync();

            var updatedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            updatedUser!.LastName.Should().Be("Lopez");

            // Delete
            context.Users.Remove(updatedUser);
            await context.SaveChangesAsync();

            var deletedUser = await context.Users.FirstOrDefaultAsync(u => u.Id == user.Id);
            deletedUser.Should().BeNull();
        }
    }

}
