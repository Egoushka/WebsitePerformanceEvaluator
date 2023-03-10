using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.Core.Interfaces.Managers;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;
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

        services.AddTransient<IClientService, ClientService>();
        services.AddTransient<ISitemapService, SitemapService>();
        services.AddTransient<ILinkManager, LinkManager>();
        services.AddTransient<Application>();
        services.AddTransient<TaskRunner>();

        var builder = new ContainerBuilder();

        builder.Populate(services);
        return builder.Build();
    }

    public static async Task Main()
    {
        await CompositionRoot().Resolve<Application>().Run();
    }
}