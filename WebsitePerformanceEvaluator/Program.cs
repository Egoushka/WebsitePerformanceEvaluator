using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.Core;

namespace WebsitePerformanceEvaluator;

internal static class Program
{
    public static async Task Main()
    {
        await CompositionRoot().GetService<TaskRunner>().Run();
    }
    
    private static ServiceProvider CompositionRoot()
    {
        var services = ConfigureServices();
        var builder = services.BuildServiceProvider();

        return builder;
    }
    
    private static ServiceCollection ConfigureServices()
    {
        var services = new ServiceCollection();

        services.ConfigureConsoleServices();
        services.ConfigureCoreServices();

        return services;
    }
}