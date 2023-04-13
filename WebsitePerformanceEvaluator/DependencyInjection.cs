using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.Core.Interfaces;

namespace WebsitePerformanceEvaluator;

public static class DependencyInjection
{
    public static void ConfigureConsoleServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddTransient<TaskRunner>();
        services.AddTransient<ConsoleWrapper>();
        services.AddTransient<ConsoleHelper>();
        services.AddTransient<ILogger, ConsoleLogger>();
    }
}