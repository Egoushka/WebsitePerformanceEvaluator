using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.API.Core.Services;
using WebsitePerformanceEvaluator.API.Core.Validators;

namespace WebsitePerformanceEvaluator.API.Core;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureAPICoreServices(this IServiceCollection services)
    {
        services.AddScoped<LinkService>();
        services.AddScoped<LinkPerformanceService>();
        services.AddScoped<UrlValidator>();

        return services;
    }
}