using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Domain.Entities;
using SupportDesk.Persistence.IntegrationTests.Helpers;
using Xunit;

namespace SupportDesk.Persistence.IntegrationTests.Repositories
{
    public class RequestRepositoryTests
    {
        [Fact]
        public async Task CanCreateRequestWithRequestType()
        {
            using var context = DbContextHelper.CreateInMemoryDbContext();

            // Ensure there is at least one RequestType in the database
            var requestType = await context.RequestTypes.FirstOrDefaultAsync(rt => rt.Description == "Tecnología de la información");
            requestType.Should().NotBeNull();

            var request = new Request
            {
                RequestTypeId = requestType!.RequestTypeId,
                ZoneId = 1,
                Comments = "This is a test request",
                RequestStatusId = 1
            };

            // Create
            await context.Requests.AddAsync(request);
            await context.SaveChangesAsync();

            // Read with include
            var fetchedRequest = await context.Requests
                .Include(r => r.RequestType)
                .FirstOrDefaultAsync(r => r.Id == request.Id);

            fetchedRequest.Should().NotBeNull();
            fetchedRequest!.RequestType.Description.Should().Be("Tecnología de la información");
        }
    }

}
