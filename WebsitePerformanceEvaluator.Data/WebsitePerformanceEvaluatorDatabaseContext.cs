using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebsitePerformanceEvaluator.Data.Configurations;
using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator.Data;

public class WebsitePerformanceEvaluatorDatabaseContext : DbContext
{
    public WebsitePerformanceEvaluatorDatabaseContext(
        DbContextOptions<WebsitePerformanceEvaluatorDatabaseContext> options) : base(options)
    {
    }

    public DbSet<LinkPerformance> LinkPerformances { get; set; }
    public DbSet<Link> Links { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(LinkConfiguration).Assembly);
    }
}