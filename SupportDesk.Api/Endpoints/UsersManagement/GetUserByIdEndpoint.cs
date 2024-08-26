using MediatR;
using SupportDesk.Api.Auth;
using SupportDesk.Application.Features.UsersManagement.Queries.GetUserById;

namespace SupportDesk.Api.Endpoints.UsersManagement;

public static class GetUserByIdEndpoint
{
    public const string Name = "GetUserById";

    public static IEndpointRouteBuilder MapGetUserById(this IEndpointRouteBuilder app)
    {
        app.MapGet(ApiEndpoints.UsersManagement.GetUserById, async (
            Guid userId,
            IMediator mediator,
            CancellationToken token) =>
        {
            var query = new GetUserByIdQuery { UserId = userId };

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
        .RequireAuthorization(AuthConstants.AdminUserPolicyName);

        return app;
    }
}
