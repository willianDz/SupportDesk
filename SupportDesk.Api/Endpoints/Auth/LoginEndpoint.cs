using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupportDesk.Application.Features.Auth.Commands.Login;

namespace SupportDesk.Api.Endpoints.Auth;

public static class LoginEndpoint
{
    public const string Name = "Login";

    public static IEndpointRouteBuilder MapLogin(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Auth.Login, async (
            [FromBody] LoginCommand command,
            IMediator mediator,
            CancellationToken token) =>
        {
            var response = await mediator.Send(command, token);

            if (!response.Success)
            {
                return Results.BadRequest(new { response.Message });
            }

            if (response.RequiresTwoFactor)
            {
                return Results.Ok(new { response.RequiresTwoFactor });
            }

            return Results.Ok(response);
        })
        .WithName(Name)
        .Produces<LoginCommandResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
