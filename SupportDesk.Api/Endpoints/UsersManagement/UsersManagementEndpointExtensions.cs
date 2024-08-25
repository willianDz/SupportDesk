namespace SupportDesk.Api.Endpoints.UsersManagement;

public static class UsersManagementEndpointExtensions
{
    public static IEndpointRouteBuilder MapUsersManagementEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateUser();
        app.MapUpdateUser();

        return app;
    }
}
