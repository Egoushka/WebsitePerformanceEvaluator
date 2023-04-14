using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WebsitePerformanceEvaluator.Data.Interfaces.Repositories;
using WebsitePerformanceEvaluator.Data.Repository;

namespace WebsitePerformanceEvaluator.Data;

public static class DependencyInjection
{
    public static IServiceCollection ConfigureDataServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<WebsitePerformanceEvaluatorDatabaseContext>(options => options.UseSqlServer(connectionString));
        
        services.AddScoped<ILinkPerformanceRepository, LinkPerformanceBaseRepository>();
        services.AddScoped<ILinkRepository, LinkBaseRepository>();

        return services;
    }
}