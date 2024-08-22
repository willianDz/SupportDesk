using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupportDesk.Api.Auth;
using SupportDesk.Application.Features.Requests.Commands.InactivateRequest;

namespace SupportDesk.Api.Endpoints.Requests;

public static class InactivateRequestEndpoint
{
    public const string Name = "InactivateRequest";

    public static IEndpointRouteBuilder MapInactivateRequest(this IEndpointRouteBuilder app)
    {
        app.MapDelete(ApiEndpoints.Requests.InactivateRequest, async (
            [FromQuery] int requestId,
            IMediator mediator,
            HttpContext httpContext,
            CancellationToken token) =>
        {
            var userId = httpContext.GetUserId();
            if (userId == null)
            {
                return Results.Unauthorized();
            }

            var command = new InactivateRequestCommand
            {
                RequestId = requestId,
                UserId = userId.Value
            };

            var response = await mediator.Send(command, cancellationToken: token);

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
            .Produces<InactivateRequestCommandResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName);

        return app;
    }
}
