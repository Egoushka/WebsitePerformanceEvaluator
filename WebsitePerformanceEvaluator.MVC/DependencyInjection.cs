using WebsitePerformanceEvaluator.MVC.Services;
using ILogger = WebsitePerformanceEvaluator.Infrastructure.Interfaces.ILogger;

namespace WebsitePerformanceEvaluator.MVC;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureMVCServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<ILogger>(new Logger("log.txt"));
        services.AddTransient<LinkService>();

        return services;
    }
}