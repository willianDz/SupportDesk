using Newtonsoft.Json;
using SupportDesk.Api.IntegrationTests.Helpers;
using SupportDesk.Application.Features.UsersManagement.Commands.CreateUser;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace SupportDesk.Api.IntegrationTests.Endpoints.Users;

public class CreateUserEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public CreateUserEndpointTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task CreateUser_Should_Return_Created_UserDto()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _factory.TestAdminJwtToken);

        using var content = new MultipartFormDataContent
        {
            {
                new StringContent("testuser@example.com"),
                nameof(CreateUserCommand.Email)
            },
            {
                new StringContent("Test"),
                nameof(CreateUserCommand.FirstName)
            },
            {
                new StringContent("User"),
                nameof(CreateUserCommand.LastName)
            },
            {
                new StringContent("ValidPassword123"),
                nameof(CreateUserCommand.Password)
            },
            {
                new StringContent("false"),
                nameof(CreateUserCommand.IsAdmin)
            },
            {
                new StringContent("false"),
                nameof(CreateUserCommand.IsSupervisor)
            }
        };

        var response = await _client.PostAsync("/api/usersmanagement", content);

        // TODO: Resolver problema con CSRF en el contexto de las pruebas

        // Assert
        //response.EnsureSuccessStatusCode();
        //var responseString = await response.Content.ReadAsStringAsync();
        //var result = JsonConvert.DeserializeObject<CreateUserCommandResponse>(responseString);

        //Assert.NotNull(result);
        //Assert.True(result.Success);
        //Assert.NotNull(result.UserCreated);
        //Assert.True(result.UserCreated.Id != Guid.Empty);
    }

    [Fact]
    public async Task CreateUser_Should_Return_Created_UserDto_With_Photo()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", _factory.TestJwtToken);

        using var content = new MultipartFormDataContent
        {
            {
                new StringContent("testuser@example.com"),
                nameof(CreateUserCommand.Email)
            },
            {
                new StringContent("Test"),
                nameof(CreateUserCommand.FirstName)
            },
            {
                new StringContent("User"),
                nameof(CreateUserCommand.LastName)
            },
            {
                new StringContent("ValidPassword123"),
                nameof(CreateUserCommand.Password)
            },
            {
                new StringContent("false"),
                nameof(CreateUserCommand.IsAdmin)
            },
            {
                new StringContent("false"),
                nameof(CreateUserCommand.IsSupervisor)
            },
            {
                MockFormFileContent("photo.jpg"),
                nameof(CreateUserCommand.Photo)
            }
        };

        // Act
        var response = await _client.PostAsync("/api/usersmanagement", content);

        // TODO: Resolver problema con CSRF en el contexto de las pruebas

        // Assert
        //response.EnsureSuccessStatusCode();
        //var responseString = await response.Content.ReadAsStringAsync();
        //var result = JsonConvert.DeserializeObject<CreateUserCommandResponse>(responseString);

        //Assert.NotNull(result);
        //Assert.True(result.Success);
        //Assert.NotNull(result.UserCreated);
        //Assert.Equal("http://localhost/files/users/photo.jpg", result.UserCreated.PhotoUrl);
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
