using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WebsitePerformanceEvaluator.Core;
using WebsitePerformanceEvaluator.Crawler;
using WebsitePerformanceEvaluator.Data;
using WebsitePerformanceEvalutor.Console.Core.Helpers;

namespace WebsitePerformanceEvaluator.InfrastructureIoC;

public static class DependencyContainer
{
    public static IServiceCollection ConfigureWebServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddSingleton(sp =>
            sp.GetRequiredService<ILoggerFactory>()
                .CreateLogger("DefaultLogger"));

        return services;
    }

    public static IServiceCollection ConfigureDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<WebsitePerformanceEvaluatorDatabaseContext>(options => options
            .UseLazyLoadingProxies()
            .UseSqlServer(connectionString));

        return services;
    }
    public static IServiceCollection ConfigureCoreServices(this IServiceCollection services)
    {
        services.AddCoreServices();
        services.AddCrawlerServices();
        
        return services;
    }
    public static IServiceCollection ConfigureConsoleServices(this IServiceCollection services)
    {
        services.AddHttpClient();
        services.AddTransient<ConsoleWrapper>();
        services.AddTransient<ConsoleHelper>();
        services.AddLogging(builder => builder.AddSimpleConsole(options => {
            options.IncludeScopes = true;
            options.SingleLine = true;
            options.TimestampFormat = "HH:mm:ss ";
        }));

        return services;
    }
}