namespace WebsitePerformanceEvaluator.Domain.Models;

public class Link
{
    public Link()
    {
        LinkPerformances = new HashSet<LinkPerformance>();
    }

    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string Url { get; set; }
    public virtual ICollection<LinkPerformance> LinkPerformances { get; set; }
}