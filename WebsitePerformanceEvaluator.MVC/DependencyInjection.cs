using ILogger = WebsitePerformanceEvaluator.Infrustructure.Interfaces.ILogger;

namespace WebsitePerformanceEvaluator.MVC;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureMVCServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton<ILogger>(new Logger("../logs/log.txt"));

        return services;
    }
}