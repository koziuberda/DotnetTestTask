using Microsoft.Extensions.DependencyInjection;
using TestTask.Services.Report;

namespace TestTask.Services;

public static class ServiceCollectionExtensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddSingleton<UserLockManager>();
        services.AddScoped<MarketService>();
        services.AddScoped<ReportService>();
    }
}