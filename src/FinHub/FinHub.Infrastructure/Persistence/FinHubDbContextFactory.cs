using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FinHub.Infrastructure.Persistence;

public sealed class FinHubDbContextFactory : IDesignTimeDbContextFactory<FinHubDbContext>
{
    public FinHubDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FinHubDbContext>();

        var connectionString = args.Length > 0 ? args[0] : null;
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") 
                               ?? "Server=localhost;Database=FinHubLocalDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
        }

        if (connectionString.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase) ||
            connectionString.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) ||
            connectionString.Contains("Host=", StringComparison.OrdinalIgnoreCase))
        {
            var formatted = FormatPostgresConnectionString(connectionString);
            optionsBuilder.UseNpgsql(formatted);
        }
        else
        {
            optionsBuilder.UseSqlServer(connectionString);
        }

        return new FinHubDbContext(optionsBuilder.Options);
    }

    private static string FormatPostgresConnectionString(string cs)
    {
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

                return $"Host={host};Port={port};Database={db};Username={user};Password={pass};SSL Mode=Require;Trust Server Certificate=true;";
            }
            catch
            {
                return cs;
            }
        }
        return cs;
    }
}
