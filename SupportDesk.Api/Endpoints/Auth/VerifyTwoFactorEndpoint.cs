using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupportDesk.Application.Features.Auth.Commands.VerifyTwoFactor;

namespace SupportDesk.Api.Endpoints.Auth;

public static class VerifyTwoFactorEndpoint
{
    public const string Name = "VerifyTwoFactor";

    public static IEndpointRouteBuilder MapVerifyTwoFactor(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Auth.VerifyTwoFactor, async (
            [FromBody] VerifyTwoFactorCommand command,
            IMediator mediator,
            CancellationToken token) =>
        {
            var response = await mediator.Send(command, token);

            if (!response.Success)
            {
                return Results.BadRequest(new { response.Message });
            }

            return Results.Ok(new { response.JwtToken });
        })
        .WithName(Name)
        .Produces<VerifyTwoFactorCommandResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest);

        return app;
    }
}
