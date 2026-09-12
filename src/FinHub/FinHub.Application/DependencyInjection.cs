using FinHub.Application.Modules.Identity.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IIdentityService, IdentityService>();
        return services;
    }
}
