using Microsoft.Extensions.Options;
using SupportDesk.Api;
using SupportDesk.Api.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .ConfigureAuthentication(builder.Configuration)
    .AddAuthorization()
    .ConfigureApiVersioning();

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


