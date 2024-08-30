using MediatR;
using Microsoft.AspNetCore.Mvc;
using SupportDesk.Api.Auth;
using SupportDesk.Application.Features.Requests.Commands.CreateRequest;

namespace SupportDesk.Api.Endpoints.Requests;

public static class CreateRequestEndpoint
{
    public const string Name = "CreateRequest";

    public static IEndpointRouteBuilder MapCreateRequest(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Requests.CreateRequest, async (
            [FromForm] CreateRequestCommand command,
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
                    return Results.BadRequest(new
                    {
                        response.Message,
                        response.ValidationErrors
                    });
                }

                return Results.Ok(response);
            })
            .WithName(Name)
            .Produces<CreateRequestCommandResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status401Unauthorized)
            .RequireAuthorization(AuthConstants.TrustedMemberPolicyName)
            .DisableAntiforgery();

        return app;
    }
}
