using SupportDesk.Api.Endpoints.Users.Requests;

namespace SupportDesk.Api.Endpoints.Requests;

public static class RequestsEndpointExtensions
{
    public static IEndpointRouteBuilder MapRequestsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateRequest();
        app.MapUpdateRequest();
        app.MapInactivateRequest();
        app.MapProcessRequest();
        app.MapGetRequestById();

        return app;
    }
}
