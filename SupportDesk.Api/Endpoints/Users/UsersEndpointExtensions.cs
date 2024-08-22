using SupportDesk.Api.Endpoints.Users.Requests;

namespace SupportDesk.Api.Endpoints.Users;

public static class UsersEndpointExtensions
{
    public static IEndpointRouteBuilder MapUsersEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetMyRequests();

        return app;
    }
}
