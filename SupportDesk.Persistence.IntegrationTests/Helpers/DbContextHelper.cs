using Microsoft.EntityFrameworkCore;
using SupportDesk.Persistence.SupportDesk;

namespace SupportDesk.Persistence.IntegrationTests.Helpers
{
    public static class DbContextHelper
    {
        public static ApplicationDbContext CreateInMemoryDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite("DataSource=:memory:")
                .Options;

            var context = new ApplicationDbContext(options);
            context.Database.OpenConnection();
            context.Database.EnsureCreated();

            return context;
        }
    }

}
