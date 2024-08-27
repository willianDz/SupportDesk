using Newtonsoft.Json;
using SupportDesk.Api.IntegrationTests.Helpers;
using System.Net.Http.Headers;
using Xunit;
using Shouldly;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Features.Users.Summary;

namespace SupportDesk.Api.IntegrationTests.Endpoints.Users.Summary;

public class GetUserSummaryEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public GetUserSummaryEndpointTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task GetUserSummary_Should_Return_Summary_When_User_Exists()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _factory.TestJwtToken);

        // Act
        var response = await _client.GetAsync($"/api/users/me/summary");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GetUserSummaryQueryResponse>(responseString);

        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
    }

}
