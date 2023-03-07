using Autofac;
using WebsitePerformanceEvaluator.Core.Interfaces.Services;
using WebsitePerformanceEvaluator.Core.Services;

namespace WebsitePerformanceEvaluator;

internal static class Program
{
    private static IContainer CompositionRoot()
    {
        var builder = new ContainerBuilder();
        builder.Register(c => new HttpClient())
            .As<HttpClient>();
        builder.RegisterType<Application>();
        builder.RegisterType<ClientService>().As<IClientService>();
        builder.RegisterType<SitemapService>().As<ISitemapService>();
        
        return builder.Build();
    }

    public static void Main()
    {
        CompositionRoot().Resolve<Application>().Run();
    }
}