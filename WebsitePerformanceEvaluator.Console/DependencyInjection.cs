using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebsitePerformanceEvaluator.Console.Helpers;

namespace WebsitePerformanceEvaluator.Console;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureConsoleServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddTransient<TaskRunner>();
        services.AddTransient<ConsoleWrapper>();
        services.AddTransient<ConsoleHelper>();
        services.AddLogging(builder => builder.AddSimpleConsole(options =>
        {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
        }));
          
        return services;
    }
}