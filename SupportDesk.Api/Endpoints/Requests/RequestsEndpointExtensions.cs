namespace SupportDesk.Api.Endpoints.Requests;

public static class RequestsEndpointExtensions
{
    public static IEndpointRouteBuilder MapRequestsEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapCreateRequest();
        app.MapUpdateRequest();
        app.MapInactivateRequest();
        app.MapProcessRequest();

        return app;
    }
}
