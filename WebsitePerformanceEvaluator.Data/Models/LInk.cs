namespace WebsitePerformanceEvaluator.Data.Models;

public class Link
{
    public int Id { get; set; }
    public string Url { get; set; }
    
    public virtual ICollection<LinkPerformance> LinkPerformances { get; set; }
}