using MediatR;
using SupportDesk.Api.Auth;
using SupportDesk.Application.Features.Tokens.Commands;

namespace SupportDesk.Api.Endpoints.Auth;

public static class RefreshTokenEndpoint
{
    public const string Name = "RefreshToken";

    public static IEndpointRouteBuilder MapRefreshToken(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Auth.RefreshToken,
            async (RefreshTokenCommand command, IMediator mediator, CancellationToken token) =>
            {
                var newToken = await mediator.Send(command, cancellationToken: token);

                if (newToken == null)
                {
                    return Results.Unauthorized();
                }

                return Results.Ok(newToken);
            })
            .WithName(Name)
            .Produces<string>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);

        return app;
    }
}
