using FinHub.Application.Common.Interfaces;
using FinHub.Infrastructure.Persistence;
using FinHub.Infrastructure.Security;
using Microsoft.Extensions.DependencyInjection;

namespace FinHub.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddSingleton<ICustomerRepository, CustomerRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();
        services.AddSingleton<ITotpService, TotpService>();

        return services;
    }
}
