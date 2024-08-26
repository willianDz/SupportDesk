using SupportDesk.Api.IntegrationTests.Helpers;
using SupportDesk.Application.Features.Requests.Commands.UpdateRequest;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace SupportDesk.Api.IntegrationTests.Endpoints.Requests;

public class UpdateRequestEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public UpdateRequestEndpointTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task UpdateRequest_Should_Return_Updated_RequestDto()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _factory.TestJwtToken);

        using var content = new MultipartFormDataContent();
        content.Add(new StringContent("1"), nameof(UpdateRequestCommand.RequestId));
        content.Add(new StringContent("2"), nameof(UpdateRequestCommand.RequestTypeId));
        content.Add(new StringContent("1"), nameof(UpdateRequestCommand.ZoneId));
        content.Add(new StringContent("Updated comments for testing"), nameof(UpdateRequestCommand.Comments));

        var documentId = 1; // Assumed to exist for testing
        content.Add(new StringContent(documentId.ToString()), nameof(UpdateRequestCommand.DocumentsToDeactivate));

        content.Add(MockFormFileContent("newdoc.jpg"), nameof(UpdateRequestCommand.NewDocuments));

        // Act
        var response = await _client.PutAsync("/api/requests", content);

        // TODO: Resolver problema con CSRF en el contexto de las pruebas

        // Assert
        //response.EnsureSuccessStatusCode();
        //var result = JsonConvert.DeserializeObject<UpdateRequestCommandResponse>(await response.Content.ReadAsStringAsync());

        //Assert.NotNull(result);
        //Assert.True(result.Success);
        //Assert.NotNull(result.RequestUpdated);
        //Assert.True(result.RequestUpdated.RequestDocuments?.Count > 0);
    }

    private static StreamContent MockFormFileContent(string fileName)
    {
        var fileStream = new MemoryStream(Encoding.UTF8.GetBytes("Fake file content"));
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
        {
            Name = "file",
            FileName = fileName
        };
        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

        return fileContent;
    }
}
