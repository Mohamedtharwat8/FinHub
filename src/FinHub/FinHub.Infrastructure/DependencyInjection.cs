using FinHub.Application.Common.Interfaces;
using FinHub.Infrastructure.Persistence;
using FinHub.Infrastructure.Security;
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
            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                options.UseSqlServer(connectionString);
            }
            else
            {
                options.UseInMemoryDatabase("FinHubCloudDb");
            }
        });

        services.AddScoped<ICustomerRepository, EfCustomerRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<ITotpService, TotpService>();
        services.AddSingleton<IOtpService, OtpService>();
        services.AddSingleton<IOAuthService, OAuthService>();

        return services;
    }
}
