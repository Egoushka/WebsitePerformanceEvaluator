namespace WebsitePerformanceEvaluator.Core.Interfaces.Managers;

public interface ILinkManager
{
    public IEnumerable<string> GetSitemapLinks(string url);
    public IEnumerable<string> GetLinksByCrawling(string url);
    public IEnumerable<Tuple<string, int>> GetLinksWithTimeResponse(string url);

}