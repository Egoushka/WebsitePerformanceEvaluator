using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.Console.Core.Helpers;

namespace WebsitePerformanceEvaluator.Console.Core;

public static class DependencyInjection
{
    public static ServiceCollection AddConsoleCoreServices(this ServiceCollection services)
    {
        services.AddTransient<ConsoleWrapper>();
        services.AddTransient<ConsoleHelper>();

        return services;
    }
}