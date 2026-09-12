using FinHub.Application;
using FinHub.Infrastructure;
using FinHub.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

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

// Auto-initialize cloud and local database schemas on startup (PostgreSQL / SQL Server)
try
{
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<FinHubDbContext>();

    if (dbContext.Database.IsNpgsql())
    {
        var pgSql = @"
CREATE TABLE IF NOT EXISTS ""Customers"" (
    ""Id"" uuid NOT NULL,
    ""Email"" varchar(256) NOT NULL,
    ""FullName"" varchar(150) NOT NULL,
    ""NationalId"" varchar(10) NULL,
    ""Address_BuildingNumber"" varchar(20) NULL,
    ""Address_Street"" varchar(100) NULL,
    ""Address_District"" varchar(100) NULL,
    ""Address_City"" varchar(100) NULL,
    ""Address_PostalCode"" varchar(10) NULL,
    ""Address_AdditionalNumber"" varchar(10) NULL,
    ""Address_Country"" varchar(50) NULL,
    ""IsCitizen"" boolean NOT NULL DEFAULT false,
    ""IsEmailVerified"" boolean NOT NULL DEFAULT false,
    ""PhoneNumber"" varchar(20) NULL,
    ""IsPhoneNumberVerified"" boolean NOT NULL DEFAULT false,
    ""PasswordHash"" varchar(4000) NOT NULL,
    ""Role"" varchar(50) NOT NULL DEFAULT 'Customer',
    ""IsMfaEnabled"" boolean NOT NULL DEFAULT false,
    ""MfaSecret"" varchar(4000) NULL,
    ""RefreshToken"" varchar(4000) NULL,
    ""RefreshTokenExpiryTime"" timestamptz NULL,
    ""AccessFailedCount"" integer NOT NULL DEFAULT 0,
    ""LockoutEnd"" timestamptz NULL,
    ""CreatedAt"" timestamptz NOT NULL,
    ""UpdatedAt"" timestamptz NULL,
    CONSTRAINT ""PK_Customers"" PRIMARY KEY (""Id"")
);

CREATE TABLE IF NOT EXISTS ""ExternalLogin"" (
    ""Id"" uuid NOT NULL,
    ""CustomerId"" uuid NOT NULL,
    ""Provider"" varchar(50) NOT NULL,
    ""ProviderKey"" varchar(100) NOT NULL,
    ""Email"" varchar(256) NULL,
    ""LinkedAt"" timestamptz NOT NULL,
    CONSTRAINT ""PK_ExternalLogin"" PRIMARY KEY (""Id""),
    CONSTRAINT ""FK_ExternalLogin_Customers_CustomerId"" FOREIGN KEY (""CustomerId"") REFERENCES ""Customers"" (""Id"") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS ""BankAccounts"" (
    ""Id"" uuid NOT NULL,
    ""CustomerId"" uuid NOT NULL,
    ""Iban"" varchar(34) NOT NULL,
    ""AccountNumber"" varchar(20) NOT NULL,
    ""Type"" varchar(20) NOT NULL,
    ""Status"" varchar(20) NOT NULL,
    ""Balance_Amount"" numeric(18,2) NOT NULL,
    ""Balance_Currency"" varchar(3) NOT NULL,
    ""CreatedAt"" timestamptz NOT NULL,
    ""UpdatedAt"" timestamptz NULL,
    CONSTRAINT ""PK_BankAccounts"" PRIMARY KEY (""Id"")
);

CREATE TABLE IF NOT EXISTS ""Transactions"" (
    ""Id"" uuid NOT NULL,
    ""AccountId"" uuid NOT NULL,
    ""Type"" varchar(20) NOT NULL,
    ""Amount"" numeric(18,2) NOT NULL,
    ""Currency"" varchar(3) NOT NULL,
    ""Description"" varchar(256) NULL,
    ""ReferenceNumber"" varchar(50) NOT NULL,
    ""TransactionDate"" timestamptz NOT NULL,
    CONSTRAINT ""PK_Transactions"" PRIMARY KEY (""Id""),
    CONSTRAINT ""FK_Transactions_BankAccounts_AccountId"" FOREIGN KEY (""AccountId"") REFERENCES ""BankAccounts"" (""Id"") ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS ""BudgetSnapshots"" (
    ""Id"" uuid NOT NULL,
    ""CustomerId"" uuid NOT NULL,
    ""Month"" timestamptz NOT NULL,
    ""MonthlyBudget"" numeric(18,2) NOT NULL,
    ""MonthlySpent"" numeric(18,2) NOT NULL,
    ""RemainingBudget"" numeric(18,2) NOT NULL,
    ""SavingsGoal"" numeric(18,2) NOT NULL,
    ""Currency"" varchar(3) NOT NULL,
    ""CreatedAt"" timestamptz NOT NULL,
    ""UpdatedAt"" timestamptz NULL,
    CONSTRAINT ""PK_BudgetSnapshots"" PRIMARY KEY (""Id"")
);
";
        dbContext.Database.ExecuteSqlRaw(pgSql);
    }
    else
    {
        dbContext.Database.EnsureCreated();
    }
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
