namespace SupportDesk.Api.Endpoints.WeatherForecast;

public static class GetWeatherForecastEndpoint
{
    public const string Name = "GetWeatherForecast";

    public static IEndpointRouteBuilder MapGetWeatherForecast(this IEndpointRouteBuilder app) 
    {
        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet(
            ApiEndpoints.WeatherForecast.GetAll, 
            (CancellationToken token) => {
            
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            
            return forecast;
        })
        .WithName($"{Name}")
        .Produces<WeatherForecast[]>(StatusCodes.Status200OK)
        .WithApiVersionSet(ApiVersioning.VersionSet)
        .HasApiVersion(1.0);

        return app;
    }
}

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
