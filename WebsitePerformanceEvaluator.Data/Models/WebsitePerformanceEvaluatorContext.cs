using Microsoft.EntityFrameworkCore;

namespace WebsitePerformanceEvaluator.Data.Models;

public class WebsitePerformanceEvaluatorContext : DbContext
{
    public DbSet<LinkPerformance> LinkPerformances { get; set; }
    
    public WebsitePerformanceEvaluatorContext(DbContextOptions<WebsitePerformanceEvaluatorContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}