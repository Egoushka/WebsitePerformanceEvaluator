namespace WebsitePerformanceEvaluator.Core.Interfaces.Services;

public interface ISitemapService
{
    public IEnumerable<string> GetAllUrlsFromSitemap(string baseUrl);
}