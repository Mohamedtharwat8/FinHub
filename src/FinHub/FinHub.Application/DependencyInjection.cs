using FinHub.Application.Modules.Banking.Services;
using FinHub.Application.Modules.Budgeting.Services;
using FinHub.Application.Modules.Customer.Services;
using FinHub.Application.Modules.Identity.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FinHub.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IIdentityService, IdentityService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IBankingService, BankingService>();
        services.AddScoped<IBudgetingService, BudgetingService>();
        return services;
    }
}
