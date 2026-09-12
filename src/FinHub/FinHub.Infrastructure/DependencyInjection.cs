using FinHub.Application.Common.Interfaces;
using FinHub.Infrastructure.Persistence;
using FinHub.Infrastructure.Security;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace FinHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<FinHubDbContext>(options =>
        {
            if (IsPostgresConnectionString(connectionString, out var pgConnectionString))
            {
                options.UseNpgsql(pgConnectionString);
            }
            else if (IsValidSqlServerConnectionString(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                options.UseInMemoryDatabase("FinHubCloudDb");
            }
        });

        services.AddScoped<ICustomerRepository, EfCustomerRepository>();
        services.AddScoped<IBankAccountRepository, EfBankAccountRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<ITotpService, TotpService>();
        services.AddSingleton<IOtpService, OtpService>();
        services.AddSingleton<IOAuthService, OAuthService>();

        return services;
    }

    private static bool IsPostgresConnectionString(string? cs, out string formatted)
    {
        formatted = string.Empty;
        if (string.IsNullOrWhiteSpace(cs)) return false;

        if (cs.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) || cs.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase))
        {
            try
            {
                var uri = new Uri(cs);
                var userInfo = uri.UserInfo.Split(':');
                var user = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : "";
                var pass = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : "";
                var host = uri.Host;
                var port = uri.Port > 0 ? uri.Port : 5432;
                var db = uri.AbsolutePath.TrimStart('/');

                formatted = $"Host={host};Port={port};Database={db};Username={user};Password={pass};SSL Mode=Require;Trust Server Certificate=true;";
                return true;
            }
            catch
            {
                formatted = cs;
                return true;
            }
        }

        if (cs.Contains("Host=", StringComparison.OrdinalIgnoreCase) && cs.Contains("Username=", StringComparison.OrdinalIgnoreCase))
        {
            formatted = cs;
            return true;
        }

        return false;
    }

    private static bool IsValidSqlServerConnectionString(string? cs)
    {
        if (string.IsNullOrWhiteSpace(cs)) return false;
        try
        {
            var builder = new SqlConnectionStringBuilder(cs);
            return !string.IsNullOrWhiteSpace(builder.DataSource);
        }
        catch
        {
            return false;
        }
    }
}
