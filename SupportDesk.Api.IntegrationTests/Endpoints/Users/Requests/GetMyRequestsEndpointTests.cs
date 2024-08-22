using Newtonsoft.Json;
using SupportDesk.Api.IntegrationTests.Helpers;
using SupportDesk.Application.Features.Users.Requests.Queries.GetMyRequests;
using System.Net.Http.Headers;
using Xunit;

namespace SupportDesk.Api.IntegrationTests.Endpoints.Users.Requests;

public class GetMyRequestsEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public GetMyRequestsEndpointTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task GetMyRequests_Should_Return_Paged_RequestDtos()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _factory.TestJwtToken);

        var queryParameters = "?page=1&pageSize=10";
        var url = $"/api/users/me/requests{queryParameters}";

        // Act
        var response = await _client.GetAsync(url);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GetMyRequestsQueryResponse>(responseString);

        Assert.NotNull(result);
    }
}
