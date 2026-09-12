using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace FinHub.Infrastructure.Persistence;

public sealed class FinHubDbContextFactory : IDesignTimeDbContextFactory<FinHubDbContext>
{
    public FinHubDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FinHubDbContext>();

        // Default local SQL Server connection string for EF Core Design-Time tooling
        var connectionString = "Server=(localdb)\\mssqllocaldb;Database=FinHubLocalDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;";
        optionsBuilder.UseSqlServer(connectionString);

        return new FinHubDbContext(optionsBuilder.Options);
    }
}
