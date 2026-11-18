using Microsoft.AspNetCore.Http.HttpResults;
using nunit_test_dashboard.Services;
using System.Text.Json.Serialization;
using System.Xml;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddAntiforgery();
builder.Services.AddScoped<NUnitXmlParser>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapPost("/api/results/upload", (IFormFile? request, NUnitXmlParser parser) =>
{
    try
    {
        if (request == null || request.Length == 0) return Results.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "File Error.");

        using var stream = request.OpenReadStream();
        if (stream == null) return Results.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "No file uploaded.");
        if (stream.Length >= 2_000_000) return Results.Problem(statusCode: StatusCodes.Status413PayloadTooLarge, detail: "File too large.");
        var result = parser.Parse(stream);
        return Results.Ok(result);
    }
    catch (InvalidDataException ex)
    {
        return Results.Problem(statusCode: StatusCodes.Status400BadRequest, detail: ex.Message);
    }
    catch (XmlException)
    {
        return Results.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Malformed XML.");
    }
    catch (Exception)
    {
        return Results.Problem(statusCode: StatusCodes.Status500InternalServerError, detail: "Internal Server Error.");
    }
}).DisableAntiforgery();

app.Run();
