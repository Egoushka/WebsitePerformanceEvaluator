using Autofac;
using WebsitePerformanceEvaluator;

internal static class Program
{
    private static IContainer CompositionRoot()
    {
        var builder = new ContainerBuilder();

        builder.RegisterType<Application>();

        //builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());
        
        return builder.Build();
    }

    public static void Main()
    {
        CompositionRoot().Resolve<Application>().Run();
    }
}