using FinHub.Application;
using FinHub.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Configure dynamic PORT binding for Cloud Hosts (Render, Heroku, Azure Container Apps)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseCors("AllowAngularApp");
app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    Status = "Online",
    Service = "FinHub Open Banking & PFM API",
    Framework = ".NET 10 (C# 14)",
    Environment = app.Environment.EnvironmentName,
    Timestamp = DateTimeOffset.UtcNow
}));

app.MapControllers();

app.Run();

public partial class Program { }
