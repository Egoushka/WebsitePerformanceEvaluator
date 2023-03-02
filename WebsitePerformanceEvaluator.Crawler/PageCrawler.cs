namespace WebsitePerformanceEvaluator.Crawler;

using HtmlAgilityPack;

public static class PageCrawler
{
    public static List<string> GetLinks(string url)
    {
        var doc = GetDocument(url);
        var linkNodes = doc.DocumentNode.SelectNodes("//a[@href]");
        if(linkNodes == null)
            return new List<string>();
        var baseUri = new Uri(url);
        
        return linkNodes.Select(link => 
                link.Attributes["href"].Value)
            .Where(href => href.StartsWith('/'))
            .Distinct()
            .Select(href => new Uri(baseUri, href).AbsoluteUri)
            .ToList();
    }
    static HtmlDocument GetDocument(string url)
    {
        var web = new HtmlWeb();
        var doc = web.Load(url);
        return doc;
    }
}