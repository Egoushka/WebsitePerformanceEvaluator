using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.Web.Core.Services;
using WebsitePerformanceEvaluator.Web.Core.Validators;

namespace WebsitePerformanceEvaluator.Web.Core;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureWebCoreServices(this IServiceCollection services)
    {
        services.AddScoped<LinkService>();
        services.AddScoped<LinkPerformanceService>();
        services.AddScoped<UrlValidator>();

        return services;
    }
}