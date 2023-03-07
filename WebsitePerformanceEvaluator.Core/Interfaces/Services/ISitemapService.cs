namespace WebsitePerformanceEvaluator.Core.Interfaces.Services;

public interface ISitemapService
{
    public List<string> GetAllUrlsFromSitemap(string baseUrl);
}