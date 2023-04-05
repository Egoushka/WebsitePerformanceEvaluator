using Autofac;
using Autofac.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Parsers;
using WebsitePerformanceEvaluator.Core.Service;
using WebsitePerformanceEvaluator.Core.Validators;

namespace WebsitePerformanceEvaluator;

internal static class Program
{
    private static IContainer CompositionRoot()
    {
        var services = new ServiceCollection();

        services.AddOptions();
        services.AddHttpClient();
        
        services.AddTransient<Crawler>();
        services.AddTransient<WebsiteCrawler>();
        services.AddTransient<SitemapCrawler>();
        services.AddTransient<HttpClientService>();
        services.AddTransient<HtmlParser>();
        services.AddTransient<XmlParser>();
        services.AddTransient<LinkFilter>();
        services.AddTransient<LinkValidator>();
        services.AddTransient<Application>();
        services.AddTransient<TaskRunner>();
        services.AddTransient<ILogger, ConsoleLogger>();

        var builder = new ContainerBuilder();
        
        builder.Populate(services);
        
        return builder.Build();
    }

    public static async Task Main()
    {
        await CompositionRoot().Resolve<Application>().Run();
    }
}