using SupportDesk.Api.Endpoints.Auth;
using SupportDesk.Api.Endpoints.Requests;
using SupportDesk.Api.Endpoints.WeatherForecast;

namespace SupportDesk.Api.Endpoints;

public static class EndpointsExtensions
{
    public static IEndpointRouteBuilder MapApiEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapAuthEndpoints();
        app.MapRequestsEndpoints();
        app.MapWeatherForecastEndpoints();
        return app;
    }
}