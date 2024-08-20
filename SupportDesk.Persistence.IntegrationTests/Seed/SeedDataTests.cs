using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Persistence.IntegrationTests.Helpers;
using Xunit;

namespace SupportDesk.Persistence.IntegrationTests.Seed
{
    public class SeedDataTests
    {
        [Fact]
        public async Task SeededRequestTypesShouldExist()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            var requestTypes = await context.RequestTypes.ToListAsync();

            requestTypes.Should().NotBeEmpty();
            requestTypes.Count.Should().Be(5); // Asumiendo que se ha seedado 5 tipos de solicitud
        }
    }
}
