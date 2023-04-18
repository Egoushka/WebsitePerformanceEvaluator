using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.MVC.Core.Services;

namespace WebsitePerformanceEvaluator.MVC.Core;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureMVCCoreServices(this IServiceCollection services)
    {
        services.AddScoped<LinkService>();
        
        return services;
    }
}