using ILogger = WebsitePerformanceEvaluator.Core.Interfaces.ILogger;

namespace WebsitePerformanceEvaluator.MVC;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureMVCServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<ILogger, Logger>();

        return services;
    }
}