using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.Core.Interfaces;

namespace WebsitePerformanceEvaluator.Console;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureConsoleServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddTransient<TaskRunner>();
        services.AddTransient<ConsoleWrapper>();
        services.AddTransient<ConsoleHelper>();
        services.AddTransient<ILogger, ConsoleLogger>();
        
        return services;
    }
}