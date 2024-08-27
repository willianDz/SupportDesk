using MediatR;
using Microsoft.AspNetCore.Http;
using SupportDesk.Api.Auth;
using SupportDesk.Application.Features.Users.Summary;

namespace SupportDesk.Api.Endpoints.Users.Summary;

public static class GetUserSummaryEndpoint
{
    public const string Name = "GetUserSummary";

    public static IEndpointRouteBuilder MapGetUserSummary(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Users.GetMySummary, async (
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken token) =>
        {
            var userId = httpContext.GetUserId();

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var query = new GetUserSummaryQuery { UserId = userId!.Value };

            var response = await mediator.Send(query, token);

            if (!response.Success)
            {
                return Results.BadRequest(new { response.Message });
            }

            return Results.Ok(response);
        })
        .WithName(Name)
        .Produces<GetUserSummaryQueryResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);

        return app;
    }
}
