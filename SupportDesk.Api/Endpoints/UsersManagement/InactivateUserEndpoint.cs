using MediatR;
using SupportDesk.Api.Auth;
using SupportDesk.Application.Features.UsersManagement.Commands.InactivateUser;

namespace SupportDesk.Api.Endpoints.UsersManagement;

public static class InactivateUserEndpoint
{
    public const string Name = "InactivateUser";

    public static IEndpointRouteBuilder MapInactivateUser(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.UsersManagement.InactivateUser, async (
            Guid userId,
            IMediator mediator,
            CancellationToken token) =>
        {
            var command = new InactivateUserCommand { UserId = userId };
            var response = await mediator.Send(command, token);

            if (!response.Success)
            {
                return Results.BadRequest(new
                {
                    response.Message,
                    response.ValidationErrors
                });
            }

            return Results.Ok(response);
        })
        .WithName(Name)
        .Produces<InactivateUserCommandResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(AuthConstants.AdminUserPolicyName);

        return app;
    }
}
