using Newtonsoft.Json;
using Shouldly;
using SupportDesk.Api.IntegrationTests.Helpers;
using SupportDesk.Application.Constants;
using System.Net.Http.Headers;
using Xunit;

namespace SupportDesk.Api.IntegrationTests.Endpoints.Users;

public class InactivateUserEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public InactivateUserEndpointTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task InactivateUser_Should_Return_Not_Found_Error()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _factory.TestAdminJwtToken);

        var userId = Guid.NewGuid(); // Replace with actual test user ID
        var url = $"/api/usersmanagement/{userId}";

        // Act
        var response = await _client.DeleteAsync(url);

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        var responseString = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(responseString);

        Assert.Equal(UsersMessages.UserNotFound, (string)result.message);
    }
}
