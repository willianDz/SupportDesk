using MediatR;
using SupportDesk.Api.Auth;
using SupportDesk.Application.Features.UsersManagement.Queries.GetUserById;

namespace SupportDesk.Api.Endpoints.UsersManagement;

public static class GetUserInformationEndpoint
{
    public const string Name = "GetUserInformation";

    public static IEndpointRouteBuilder MapGetUserInformation(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.Users.Profile.GetUserInformation, async (
        IMediator mediator,
            HttpContext httpContext,
            CancellationToken token) =>
        {
            var userId = httpContext.GetUserId();

            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var query = new GetUserByIdQuery { UserId = userId!.Value };

            var response = await mediator.Send(query, token);

            if (!response.Success)
            {
                return Results.BadRequest(new { response.Message });
            }

            return Results.Ok(response);
        })
        .WithName(Name)
        .Produces<GetUserByIdQueryResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);

        return app;
    }
}
