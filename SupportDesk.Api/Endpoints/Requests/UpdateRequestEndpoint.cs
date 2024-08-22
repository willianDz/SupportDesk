using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupportDesk.Api.Auth;
using SupportDesk.Application.Features.Requests.Commands.UpdateRequest;

namespace SupportDesk.Api.Endpoints.Requests;

public static class UpdateRequestEndpoint
{
    public const string Name = "UpdateRequest";

    public static IEndpointRouteBuilder MapUpdateRequest(this IEndpointRouteBuilder app)
    {
        app.MapPut(ApiEndpoints.Requests.UpdateRequest, async (
            [FromForm] UpdateRequestCommand command,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken token) =>
        {
            var userId = httpContext.GetUserId();
            if (userId == null)
            {
                return Results.Unauthorized();
            }

            command.UserId = userId.Value;

            var response = await mediator.Send(command, cancellationToken: token);

            if (!response.Success)
            {
                return Results.BadRequest(new { response.ValidationErrors });
            }

            return Results.Ok(response);
        })
            .WithName(Name)
            .Produces<UpdateRequestCommandResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);

        return app;
    }
}
