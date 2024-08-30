using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupportDesk.Api.Auth;
using SupportDesk.Application.Features.UsersManagement.Commands.CreateUser;

namespace SupportDesk.Api.Endpoints.UsersManagement;

public static class CreateUserEndpoint
{
    public const string Name = "CreateUser";

    public static IEndpointRouteBuilder MapCreateUser(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.UsersManagement.CreateUser, async (
            [FromForm] CreateUserCommand command,
            IMediator mediator,
            HttpContext httpContext,
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
        .Produces<CreateUserCommandResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(AuthConstants.AdminUserPolicyName)
        .DisableAntiforgery();

        return app;
    }
}
