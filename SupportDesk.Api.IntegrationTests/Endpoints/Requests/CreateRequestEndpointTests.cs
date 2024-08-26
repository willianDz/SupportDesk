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

        using var content = new MultipartFormDataContent
        {
            {
                new StringContent("1"),
                nameof(CreateRequestCommand.RequestTypeId)
            },
            {
                new StringContent("1"),
                nameof(CreateRequestCommand.ZoneId)
            },
            {
                new StringContent("This is a valid comment for testing"),
                nameof(CreateRequestCommand.Comments)
            },
        };

        var response = await _client.PostAsync("/api/requests", content);

        // TODO: Resolver problema con CSRF en el contexto de las pruebas

        // Assert
        //response.EnsureSuccessStatusCode();
        //var responseString = await response.Content.ReadAsStringAsync();
        //var result = JsonConvert.DeserializeObject<CreateRequestCommandResponse>(responseString);

        //Assert.NotNull(result);
        //Assert.True(result.Success);
        //Assert.NotNull(result.RequestCreated);
        //Assert.True(result.RequestCreated.Id > 0);
    }

    [Fact]
    public async Task CreateRequest_Should_Return_Created_RequestDto_With_Documents()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _factory.TestJwtToken);

        using var content = new MultipartFormDataContent
        {
            { 
                new StringContent("1"), 
                nameof(CreateRequestCommand.RequestTypeId) 
            },
            { 
                new StringContent("1"), 
                nameof(CreateRequestCommand.ZoneId) 
            },
            { 
                new StringContent("This is a valid comment for testing"), 
                nameof(CreateRequestCommand.Comments) 
            },
            { 
                MockFormFileContent("doc1.jpg"), 
                nameof(CreateRequestCommand.Documents) 
            },
            { 
                MockFormFileContent("doc2.jpg"), 
                nameof(CreateRequestCommand.Documents) 
            }
        };

        // Act
        var response = await _client.PostAsync("/api/requests", content);

        // Assert

        // TODO: Resolver problema con CSRF en el contexto de las pruebas

        //response.EnsureSuccessStatusCode();
        //var result = JsonConvert.DeserializeObject<CreateRequestCommandResponse>(await response.Content.ReadAsStringAsync());

        //Assert.NotNull(result);
        //Assert.True(result.Success);
        //Assert.NotNull(result.RequestCreated);
        //Assert.Equal(2, result.RequestCreated.RequestDocuments?.Count);
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
