using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.Core.Service;

namespace WebsitePerformanceEvaluator.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddTransient<LinkService>();
        services.AddTransient<LinkPerformanceService>();
        
        return services;
    }
    
}