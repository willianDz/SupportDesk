using MediatR;
using SupportDesk.Api.Auth;
using SupportDesk.Application.Features.Requests.Commands.ProcessRequest;

namespace SupportDesk.Api.Endpoints.Requests;

public static class ProccessRequestEndpoint
{
    public const string Name = "ProcessRequest";

    public static IEndpointRouteBuilder MapProcessRequest(this IEndpointRouteBuilder app)
    {
        app.MapPost(ApiEndpoints.Requests.ProcessRequest, async (
            ProcessRequestCommand command,
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
        .Produces<ProcessRequestCommandResponse>(StatusCodes.Status200OK)
        .Produces(StatusCodes.Status400BadRequest)
        .Produces(StatusCodes.Status401Unauthorized)
        .RequireAuthorization(AuthConstants.AdminUserPolicyName);

        return app;
    }
}
