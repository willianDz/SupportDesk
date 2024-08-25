using Microsoft.Extensions.Options;
using SupportDesk.Api.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SupportDesk.Api;

public class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configuración del archivo appsettings.json y otros archivos de configuración
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();  // Cargar las variables de entorno

        Console.WriteLine($"JWT Key: {builder.Configuration["Jwt:Key"]}");
        Console.WriteLine($"JWT Issuer: {builder.Configuration["Jwt:Issuer"]}");
        Console.WriteLine($"JWT Audience: {builder.Configuration["Jwt:Audience"]}");


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