namespace WebsitePerformanceEvaluator.Core.Interfaces.Services;

public interface ISitemapService
{
    public Task<List<string>> GetAllUrlsFromSitemapAsync(string baseUrl);
}