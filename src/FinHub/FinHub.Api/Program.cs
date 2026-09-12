using FinHub.Application;
using FinHub.Infrastructure;
using FinHub.Infrastructure.Persistence;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Configure dynamic PORT binding for Cloud Hosts (Render, Heroku, Azure Container Apps)
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// Auto-initialize cloud database schema on startup (Neon PostgreSQL / SQL Server)
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<FinHubDbContext>();
    dbContext.Database.EnsureCreated();
}
catch (Exception ex)
{
    Console.WriteLine($"[Database Auto-Init Warning]: {ex.Message}");
}

// Always enable Swagger UI in both Development and Production for easy cloud API testing
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "FinHub API v1");
    c.RoutePrefix = "swagger";
});

app.UseAuthorization();

app.MapGet("/", () => Results.Ok(new
{
    Status = "Online",
    Service = "FinHub Open Banking & PFM API",
    Framework = ".NET 10 (C# 14)",
    Environment = app.Environment.EnvironmentName,
    SwaggerUI = "/swagger",
    Timestamp = DateTimeOffset.UtcNow
}));

app.MapControllers();

app.Run();

public partial class Program { }
