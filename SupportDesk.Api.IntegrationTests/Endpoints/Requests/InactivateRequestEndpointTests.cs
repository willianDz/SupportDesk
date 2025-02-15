﻿using Newtonsoft.Json;
using SupportDesk.Api.IntegrationTests.Helpers;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Features.Requests.Commands.InactivateRequest;
using System.Net.Http.Headers;
using Xunit;

namespace SupportDesk.Api.IntegrationTests.Endpoints.Requests;

public class InactivateRequestEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public InactivateRequestEndpointTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task InactivateRequest_Should_Return_Success_Response()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _factory.TestJwtToken);

        // Act
        var response = await _client.DeleteAsync("/api/requests?requestId=1");

        // Assert
        response.EnsureSuccessStatusCode();
        var result = JsonConvert.DeserializeObject<InactivateRequestCommandResponse>(await response.Content.ReadAsStringAsync());

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.Equal(RequestMessages.RequestHasBeenInactive, result.Message);
    }

    [Fact]
    public async Task InactivateRequest_Should_Return_BadRequest_When_Request_Does_Not_Exist()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _factory.TestJwtToken);

        // Act
        var response = await _client.DeleteAsync("/api/requests?requestId=9999"); // ID que no existe

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        Assert.Equal(System.Net.HttpStatusCode.BadRequest, response.StatusCode);

        var result = JsonConvert.DeserializeObject<dynamic>(await response.Content.ReadAsStringAsync());

        Assert.NotNull(result);
        Assert.Equal(RequestMessages.RequestNotFoundOrIsInactive, (string)result.message);
    }
}
