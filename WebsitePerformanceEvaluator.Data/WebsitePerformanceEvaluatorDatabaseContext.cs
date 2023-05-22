using Microsoft.EntityFrameworkCore;
using WebsitePerformanceEvaluator.Core;
using WebsitePerformanceEvaluator.Data.Configurations;

namespace WebsitePerformanceEvaluator.Data;

public class WebsitePerformanceEvaluatorDatabaseContext : DatabaseContext
{
    public WebsitePerformanceEvaluatorDatabaseContext(
        DbContextOptions<WebsitePerformanceEvaluatorDatabaseContext> options) : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LinkConfiguration).Assembly);
    }
}