﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.Core;
using WebsitePerformanceEvaluator.Data;

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
        
        services.ConfigureConsoleServices();
        services.ConfigureCoreServices();
        services.ConfigureDataServices(configuration);

        return services;
    }
}