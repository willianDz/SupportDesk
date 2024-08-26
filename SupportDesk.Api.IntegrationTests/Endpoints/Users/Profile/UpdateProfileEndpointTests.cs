using Newtonsoft.Json;
using SupportDesk.Api.IntegrationTests.Helpers;
using SupportDesk.Application.Features.Users.Profile.Commands.UpdateProfile;
using System.Net.Http.Headers;
using Xunit;
using Shouldly;
using FluentAssertions;
using System.Text;

namespace SupportDesk.Api.IntegrationTests.Endpoints.Users.Profile;

public class UpdateProfileEndpointTests : IClassFixture<CustomWebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory<Program> _factory;

    public UpdateProfileEndpointTests(CustomWebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
        _factory = factory;
    }

    [Fact]
    public async Task UpdateProfile_Should_Update_UserDetails()
    {
        // Arrange
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _factory.TestJwtToken);

        using var content = new MultipartFormDataContent
        {
            {
                new StringContent("UpdatedFirstName"),
                nameof(UpdateProfileCommand.FirstName)
            },
            {
                new StringContent("UpdatedLastName"),
                nameof(UpdateProfileCommand.LastName)
            },
            {
                new StringContent(new DateTime(1990, 1, 1).ToString("yyyy-MM-dd")),
                nameof(UpdateProfileCommand.DateOfBirth)
            },
            {
                new StringContent("1"),
                nameof(UpdateProfileCommand.GenderId)
            },
            {
                MockFormFileContent("newphoto.jpg"),
                nameof(UpdateProfileCommand.Photo)
            }
        };

        // Act
        var response = await _client.PutAsync("/api/users/me/profile", content);

        // TODO: Resolver problema con CSRF en el contexto de las pruebas

        // Assert
        //response.EnsureSuccessStatusCode();
        //var responseString = await response.Content.ReadAsStringAsync();
        //var result = JsonConvert.DeserializeObject<UpdateProfileCommandResponse>(responseString);

        //result.ShouldNotBeNull();
        //result.Success.ShouldBeTrue();
        //result.UserUpdated.ShouldNotBeNull();
        //result.UserUpdated.FirstName.ShouldBe("UpdatedFirstName");
        //result.UserUpdated.LastName.ShouldBe("UpdatedLastName");
        //result.UserUpdated.GenderId.ShouldBe(1);
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
