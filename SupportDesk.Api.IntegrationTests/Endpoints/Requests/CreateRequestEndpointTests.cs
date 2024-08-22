using Newtonsoft.Json;
using SupportDesk.Api.IntegrationTests.Helpers;
using SupportDesk.Application.Features.Requests.Commands.CreateRequest;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace SupportDesk.Api.IntegrationTests.Endpoints.Requests;

public class CreateRequestEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public CreateRequestEndpointTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    

    [Fact]
    public async Task CreateRequest_Should_Return_Created_RequestDto()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _factory.TestJwtToken);

        var command = new CreateRequestCommand
        {
            RequestTypeId = 1,
            ZoneId = 1,
            Comments = "This is a valid comment for testing"
        };

        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/requests", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<CreateRequestCommandResponse>(responseString);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.RequestCreated);
        Assert.True(result.RequestCreated.Id > 0);
    }
}
