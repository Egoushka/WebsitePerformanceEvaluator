using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.Core.Service;
using WebsitePerformanceEvaluator.Crawler;

namespace WebsitePerformanceEvaluator.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddCrawlerServices();
        
        services.AddTransient<LinkService>();
        services.AddTransient<LinkPerformanceService>();
        
        return services;
    }
    
}