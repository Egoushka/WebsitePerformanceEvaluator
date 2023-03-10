namespace WebsitePerformanceEvaluator.Core.Interfaces.Managers;

public interface ILinkManager
{
    public IEnumerable<string> GetSitemapLinks(string url);
    public Task<IEnumerable<string>> GetLinksByCrawling(string url);
    public Task<IEnumerable<Tuple<string, int>>> GetLinksWithTimeResponse(string url);
}