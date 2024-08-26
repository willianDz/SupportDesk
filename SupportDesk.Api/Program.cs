using Microsoft.Extensions.Options;
using SupportDesk.Api.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SupportDesk.Api;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configuraci�n del archivo appsettings.json y otros archivos de configuraci�n
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

        builder.Services
            .ConfigureAuthentication(builder.Configuration)
            .ConfigureAuthorization()
            .ConfigureApiVersioning()
            .ConfigureScheduledTasks();

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.ConfigureOutputCache();
        builder.Services.ConfigureHealthChecks();

        builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        builder.Services.AddSwaggerGen(x => x.OperationFilter<SwaggerDefaultValues>());

        var app = builder
            .ConfigureServices()
            .ConfigurePipeline();

        app.Run();
    }
}