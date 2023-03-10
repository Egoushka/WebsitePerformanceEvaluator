namespace WebsitePerformanceEvaluator.Core.Interfaces.Services;

public interface ISitemapService
{
    public Task<IEnumerable<string>> GetAllUrlsFromSitemap(string baseUrl);
}