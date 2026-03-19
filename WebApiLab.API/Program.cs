using System.Reflection.Metadata;
using Scalar.AspNetCore;
using System.Text.Json;
using WebApiLab.API.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi("v1", options =>
    {
        options.AddDocumentTransformer((document, context, ct) =>
        {
            document.Info.Title = "WebApiLab API";
            document.Info.Version = context.DocumentName;
            document.Info.Description = "WebLab API using OpenApi";
            return Task.CompletedTask;
        });
    }
);

builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


var app = builder.Build();

string jsonFile = File.ReadAllText("./Resources/64KB.json");
var jsonData = JsonSerializer.Deserialize<List<Person>>(
    jsonFile, 
    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

app.MapGet("/people", () => jsonData)
    .WithName("GetPeople")
    .WithOpenApi()
    .Produces<List<Person>>(StatusCodes.Status200OK);

app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi("/openapi/{documentName}.json");
    app.MapScalarApiReference(Options =>
    {
        Options.Title = "WebApiLab API";
        Options.WithOpenApiRoutePattern("/openapi/{documentName}.json");
    });
}

app.UseHttpsRedirection();

app.UseCors();
app.Run();
