using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.Console.Core;
using WebsitePerformanceEvaluator.InfrastructureIoC;

namespace WebsitePerformanceEvaluator.Console;

internal static class Program
{
    public static async Task Main()
    {
        await CompositionRoot().GetService<TaskRunner>().RunAsync();
    }
    
    private static ServiceProvider CompositionRoot()
    {
        var configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", false, true)
            .Build();
        
        var services = ConfigureServices(configuration);

        var builder = services.BuildServiceProvider();
        
        return builder;
    }

    private static ServiceCollection ConfigureServices(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        
        services.AddConsoleCoreServices();

        services.ConfigureConsoleServices();
        services.ConfigureCoreServices();
        services.ConfigureDataServices(configuration);
        
        services.AddTransient<TaskRunner>();

        return services;
    }
}