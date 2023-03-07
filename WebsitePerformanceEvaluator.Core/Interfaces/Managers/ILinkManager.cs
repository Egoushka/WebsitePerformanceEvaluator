namespace WebsitePerformanceEvaluator.Core.Interfaces.Managers;

public interface ILinkManager
{
    public IEnumerable<string> GetLinksThatExistInSitemapButNotInCrawling(string url);
    public IEnumerable<string> GetLinksThatExistInCrawlingButNotInSitemap(string url);
    public IEnumerable<Tuple<string, int>> GetLinksWithTimeResponse(string url);

}