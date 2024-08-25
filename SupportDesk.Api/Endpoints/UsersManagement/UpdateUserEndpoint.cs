using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupportDesk.Api.Auth;
using SupportDesk.Application.Features.UsersManagement.Commands.UpdateUser;

namespace SupportDesk.Api.Endpoints.UsersManagement;

public static class UpdateUserEndpoint
{
    public const string Name = "UpdateUser";

    public static IEndpointRouteBuilder MapUpdateUser(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.UsersManagement.UpdateUser, async (
            [FromForm] UpdateUserCommand command,
            IMediator mediator,
            CancellationToken token) =>
        {
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
        .Produces<UpdateUserCommandResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(AuthConstants.AdminUserPolicyName);

        return app;
    }
}
