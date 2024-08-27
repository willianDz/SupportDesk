using SupportDesk.Api.Endpoints.Users.Profile;
using SupportDesk.Api.Endpoints.Users.Requests;
using SupportDesk.Api.Endpoints.Users.Summary;
using SupportDesk.Api.Endpoints.UsersManagement;

namespace SupportDesk.Api.Endpoints.Users;

public static class UsersEndpointExtensions
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetMyRequests();
        app.MapUpdateProfile();
        app.MapGetUserInformation();
        app.MapGetUserSummary();

        return app;
    }
}
