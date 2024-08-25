using Newtonsoft.Json;
using SupportDesk.Api.IntegrationTests.Helpers;
using SupportDesk.Application.Features.UsersManagement.Queries.GetUserById;
using System.Net.Http.Headers;
using Xunit;
using Shouldly;
using SupportDesk.Application.Constants;

namespace SupportDesk.Api.IntegrationTests.Endpoints.UsersManagement;

public class GetUserByIdEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public GetUserByIdEndpointTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task GetUserById_Should_Return_UserDetailsDto_When_User_Exists()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _factory.TestAdminJwtToken);
        var userId = Guid.NewGuid(); // Suponiendo que este es un ID de usuario existente

        // Act
        var response = await _client.GetAsync($"/api/usersmanagement/{userId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GetUserByIdQueryResponse>(responseString);

        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.User.ShouldNotBeNull();
        result.User.Id.ShouldBe(userId);
    }

    [Fact]
    public async Task GetUserById_Should_Return_Error_When_User_Not_Found()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _factory.TestAdminJwtToken);
        var nonExistentUserId = Guid.NewGuid();

        // Act
        var response = await _client.GetAsync($"/api/usersmanagement/{nonExistentUserId}");

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        var responseString = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(responseString);

        Assert.Equal(UsersMessages.UserNotFound, (string)result.message);
    }
}
