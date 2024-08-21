namespace SupportDesk.Api.Endpoints.Auth;

public static class AuthEndpointExtensions
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapRefreshToken();

        return app;
    }
}