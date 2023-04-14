using Microsoft.EntityFrameworkCore;
using WebsitePerformanceEvaluator.Data.Models;

namespace WebsitePerformanceEvaluator;

public class WebsitePerformanceEvaluatorContext : DbContext
{
    public DbSet<LinkPerformance> LinkPerformances { get; set; }
    
    public WebsitePerformanceEvaluatorContext(DbContextOptions<WebsitePerformanceEvaluatorContext> options) : base(options)
    {
        Database.EnsureCreated();
    }
}