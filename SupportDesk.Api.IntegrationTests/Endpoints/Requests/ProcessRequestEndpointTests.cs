using Newtonsoft.Json;
using SupportDesk.Api.IntegrationTests.Helpers;
using SupportDesk.Application.Constants;
using SupportDesk.Application.Features.Requests.Commands.ProcessRequest;
using SupportDesk.Domain.Enums;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace SupportDesk.Api.IntegrationTests.Endpoints.Requests;

public class ProcessRequestEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public ProcessRequestEndpointTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task ProcessRequest_Should_Return_Updated_RequestDto()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _factory.TestSupervisorJwtToken);

        var command = new ProcessRequestCommand
        {
            RequestId = 1,
            NewStatusId = (int)RequestStatusesEnum.UnderReview
        };

        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/requests/process", content);

        // Assert
        response.EnsureSuccessStatusCode();
        var responseString = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<ProcessRequestCommandResponse>(responseString);

        Assert.NotNull(result);
        Assert.True(result.Success);
        Assert.NotNull(result.ProcessedRequest);
        Assert.Equal((int)RequestStatusesEnum.UnderReview, result.ProcessedRequest.RequestStatusId);
    }

    [Fact]
    public async Task ProcessRequest_Should_Return_Error_When_Request_Not_Found()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _factory.TestSupervisorJwtToken);

        var command = new ProcessRequestCommand
        {
            RequestId = 999, // Non-existent request ID
            NewStatusId = (int)RequestStatusesEnum.UnderReview
        };

        var content = new StringContent(JsonConvert.SerializeObject(command), Encoding.UTF8, "application/json");

        // Act
        var response = await _client.PostAsync("/api/requests/process", content);

        // Assert
        Assert.False(response.IsSuccessStatusCode);
        var responseString = await response.Content.ReadAsStringAsync();
        dynamic result = JsonConvert.DeserializeObject(responseString);
        Assert.Equal(RequestMessages.RequestNotFound, (string)result.message);
    }
}
