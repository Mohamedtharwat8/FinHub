using FinHub.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinHub.Infrastructure.Persistence;

public sealed class FinHubDbContext : DbContext
{
    public DbSet<Customer> Customers => Set<Customer>();

    public FinHubDbContext(DbContextOptions<FinHubDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FinHubDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}
