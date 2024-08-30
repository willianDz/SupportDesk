using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupportDesk.Api.Auth;
using SupportDesk.Application.Features.Users.Profile.Commands.UpdateProfile;

namespace SupportDesk.Api.Endpoints.Users.Profile;

public static class UpdateProfileEndpoint
{
    public const string Name = "UpdateProfile";

    public static IEndpointRouteBuilder MapUpdateProfile(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Users.Profile.UpdateProfile, async (
            [FromForm] UpdateProfileCommand command,
            HttpContext httpContext,
            IMediator mediator,
            CancellationToken token) =>
        {
            var userId = httpContext.GetUserId();
            if (userId == null)
            {
                return Results.Unauthorized();
            }

            command.UserId = userId.Value;

            var response = await mediator.Send(command, token);

            if (!response.Success)
            {
                return Results.BadRequest(new { response.Message, response.ValidationErrors });
            }

            return Results.Ok(response);
        })
        .WithName(Name)
        .Produces<UpdateProfileCommandResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(AuthConstants.TrustedMemberPolicyName)
        .DisableAntiforgery();

        return app;
    }
}
