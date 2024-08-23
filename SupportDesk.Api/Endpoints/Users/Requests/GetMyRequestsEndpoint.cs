using MediatR;
using SupportDesk.Api.Auth;
using SupportDesk.Application.Features.Users.Requests.Queries.GetMyRequests;

namespace SupportDesk.Api.Endpoints.Users.Requests;

public static class GetMyRequestsEndpoint
{
    public const string Name = "GetMyRequests";

    public static IEndpointRouteBuilder MapGetMyRequests(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Users.GetMyRequests, async (
            [AsParameters] GetMyRequestsQuery query,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken token) =>
        {
            var userId = httpContext.GetUserId();
            if (userId == null)
            {
                return Results.Unauthorized();
            }

            query.UserId = userId.Value;

            var response = await mediator.Send(query, cancellationToken: token);

            return Results.Ok(response);
        })
        .WithName(Name)
        .Produces<GetMyRequestsQueryResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);

        return app;
    }
}
