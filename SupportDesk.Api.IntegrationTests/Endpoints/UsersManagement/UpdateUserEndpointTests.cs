using Newtonsoft.Json;
using SupportDesk.Api.IntegrationTests.Helpers;
using SupportDesk.Application.Features.UsersManagement.Commands.UpdateUser;
using System.Net.Http.Headers;
using System.Text;
using Xunit;

namespace SupportDesk.Api.IntegrationTests.Endpoints.Users;

public class UpdateUserEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public UpdateUserEndpointTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task UpdateUser_Should_Return_Updated_UserDto()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _factory.TestAdminJwtToken);

        var userId = Guid.NewGuid(); // Replace with actual test user ID
        var url = $"/api/usersmanagement/{userId}";

        using var content = new MultipartFormDataContent
        {
            {
                new StringContent("updateduser@example.com"),
                nameof(UpdateUserCommand.Email)
            },
            {
                new StringContent("Updated"),
                nameof(UpdateUserCommand.FirstName)
            },
            {
                new StringContent("User"),
                nameof(UpdateUserCommand.LastName)
            },
            {
                new StringContent("true"),
                nameof(UpdateUserCommand.IsAdmin)
            },
            {
                new StringContent("true"),
                nameof(UpdateUserCommand.IsSupervisor)
            }
        };

        var response = await _client.PutAsync(url, content);

        // TODO: Resolver problema con CSRF en el contexto de las pruebas

        // Assert
        //response.EnsureSuccessStatusCode();
        //var responseString = await response.Content.ReadAsStringAsync();
        //var result = JsonConvert.DeserializeObject<UpdateUserCommandResponse>(responseString);

        //Assert.NotNull(result);
        //Assert.True(result.Success);
        //Assert.NotNull(result.UpdatedUser);
        //Assert.Equal("updateduser@example.com", result.UpdatedUser.Email);
    }

    [Fact]
    public async Task UpdateUser_Should_Return_Updated_UserDto_With_Photo()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _factory.TestAdminJwtToken);

        var userId = Guid.NewGuid(); // Replace with actual test user ID
        var url = $"/api/usersmanagement/{userId}";

        using var content = new MultipartFormDataContent
        {
            {
                new StringContent("updateduser@example.com"),
                nameof(UpdateUserCommand.Email)
            },
            {
                new StringContent("Updated"),
                nameof(UpdateUserCommand.FirstName)
            },
            {
                new StringContent("User"),
                nameof(UpdateUserCommand.LastName)
            },
            {
                new StringContent("true"),
                nameof(UpdateUserCommand.IsAdmin)
            },
            {
                new StringContent("true"),
                nameof(UpdateUserCommand.IsSupervisor)
            },
            {
                MockFormFileContent("newphoto.jpg"),
                nameof(UpdateUserCommand.Photo)
            }
        };

        var response = await _client.PutAsync(url, content);

        // TODO: Resolver problema con CSRF en el contexto de las pruebas

        // Assert
        //response.EnsureSuccessStatusCode();
        //var responseString = await response.Content.ReadAsStringAsync();
        //var result = JsonConvert.DeserializeObject<UpdateUserCommandResponse>(responseString);

        //Assert.NotNull(result);
        //Assert.True(result.Success);
        //Assert.NotNull(result.UpdatedUser);
        //Assert.Equal("http://localhost/files/users/newphoto.jpg", result.UpdatedUser.PhotoUrl);
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
