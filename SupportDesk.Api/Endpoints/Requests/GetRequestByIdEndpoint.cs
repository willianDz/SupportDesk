using MediatR;
using SupportDesk.Api.Auth;
using SupportDesk.Application.Features.Requests.Queries.GetRequestById;

namespace SupportDesk.Api.Endpoints.Users.Requests;

public static class GetRequestByIdEndpoint
{
    public const string Name = "GetRequestById";

    public static IEndpointRouteBuilder MapGetRequestById(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Requests.GetRequestById, async (
            int id,
            IMediator mediator,
            CancellationToken token) =>
        {
            var query = new GetRequestByIdQuery { Id = id };

            var response = await mediator.Send(query, cancellationToken: token);

            if (!response.Success)
            {
                return Results.BadRequest(new { response.Message });
            }

            return Results.Ok(response);
        })
        .WithName(Name)
        .Produces<GetRequestByIdQueryResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);

        return app;
    }
}
