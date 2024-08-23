using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Persistence.SupportDesk;
using Xunit;

namespace SupportDesk.Persistence.IntegrationTests.Seed
{
    public class SeedDataTests
    {
        private readonly ApplicationDbContext _dbContext;

        public SeedDataTests()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "SupportDeskDb")
                .Options;

            _dbContext = new ApplicationDbContext(options);
            _dbContext.Database.EnsureCreated();
        }

        [Fact]
        public async Task SeededRequestTypesShouldExist()
        {
            var requestTypes = await _dbContext.RequestTypes.ToListAsync();

            requestTypes.Should().NotBeEmpty();
            requestTypes.Count.Should().Be(5); // Asumiendo que se ha seedado 5 tipos de solicitud
        }
    }
}
