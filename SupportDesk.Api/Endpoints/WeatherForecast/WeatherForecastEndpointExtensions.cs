namespace SupportDesk.Api.Endpoints.WeatherForecast;

public static class WeatherForecastEndpointExtensions
{
    public static IEndpointRouteBuilder MapWeatherForecastEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGetWeatherForecast();

        return app;
    }
}
