using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WebsitePerformanceEvaluator.Core.Managers;
using WebsitePerformanceEvaluator.Core.Services;

namespace WebsitePerformanceEvaluator;

internal static class Program
{
    private static IContainer CompositionRoot()
    {
        var services = new ServiceCollection();

        services.AddMemoryCache();
        services.AddOptions();
        services.Configure<MemoryCacheEntryOptions>(options => options.SetSlidingExpiration(TimeSpan.FromMinutes(1)));
        services.AddHttpClient();
        services.AddTransient<ClientService>();
        services.AddTransient<SitemapService>();
        services.AddTransient<LinkManager>();
        services.AddTransient<Application>();
        services.AddTransient<TaskRunner>();

        var builder = new ContainerBuilder();
        builder.Register<ILogger>((c, p) =>
            new LoggerConfiguration().WriteTo.Console().CreateLogger()
        ).SingleInstance();
        
        builder.Populate(services);
        return builder.Build();
    }

    public static async Task Main()
    {
        await CompositionRoot().Resolve<Application>().Run();
    }
}