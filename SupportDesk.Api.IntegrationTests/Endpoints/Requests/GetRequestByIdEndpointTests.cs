using Newtonsoft.Json;
using SupportDesk.Api.IntegrationTests.Helpers;
using SupportDesk.Application.Features.Requests.Queries.GetRequestById;
using System.Net.Http.Headers;
using Xunit;
using Shouldly;
using SupportDesk.Application.Constants;

namespace SupportDesk.Api.IntegrationTests.Endpoints.Requests;

public class GetRequestByIdEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public GetRequestByIdEndpointTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task GetRequestById_Should_Return_RequestDetailsDto_When_Request_Exists()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _factory.TestJwtToken);
        int requestId = 1;

        // Act
        var response = await _client.GetAsync($"/api/requests/{requestId}");

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<GetRequestByIdQueryResponse>(responseString);

        result.ShouldNotBeNull();
        result.Success.ShouldBeTrue();
        result.Request.ShouldNotBeNull();
        result.Request.Id.ShouldBe(requestId);
    }

    [Fact]
    public async Task GetRequestById_Should_Return_Error_When_Request_Not_Found()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _factory.TestJwtToken);
        int nonExistentRequestId = 999;

        // Act
        var response = await _client.GetAsync($"/api/requests/{nonExistentRequestId}");

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        var responseString = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(responseString);

        Assert.Equal(RequestMessages.RequestNotFound, (string)result.message);
    }

    [Fact]
    public async Task GetRequestById_Should_Return_Error_When_Request_Is_Inactive()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _factory.TestJwtToken);
        int inactiveRequestId = 2; // Suponiendo que la solicitud con ID 2 está inactiva

        // Act
        var response = await _client.GetAsync($"/api/requests/{inactiveRequestId}");

        // Assert
        response.StatusCode.ShouldBe(System.Net.HttpStatusCode.BadRequest);
        var responseString = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(responseString);

        Assert.Equal(RequestMessages.RequestIsInactive, (string)result.message);
    }
}
