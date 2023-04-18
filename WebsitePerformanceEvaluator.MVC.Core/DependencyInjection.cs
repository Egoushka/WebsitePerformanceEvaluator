using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.MVC.Core.Services;
using WebsitePerformanceEvaluator.MVC.Core.Validators;

namespace WebsitePerformanceEvaluator.MVC.Core;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureMVCCoreServices(this IServiceCollection services)
    {
        services.AddScoped<LinkService>();
        services.AddScoped<LinkPerformanceService>();
        services.AddScoped<UrlValidator>();

        return services;
    }
}