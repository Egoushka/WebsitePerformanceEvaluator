using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;
using WebsitePerformanceEvaluator.Core.Service;

namespace WebsitePerformanceEvaluator.Core;

public static class DependencyInjection
{
    public static IServiceCollection AddCoreServices(this IServiceCollection services)
    {
        services.AddTransient<ILinkService, LinkService>();
        services.AddTransient<ILinkPerformanceService, LinkPerformanceService>();
        
        return services;
    }
    
}