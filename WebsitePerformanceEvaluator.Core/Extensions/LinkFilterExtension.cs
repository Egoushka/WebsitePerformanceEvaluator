namespace WebsitePerformanceEvaluator.Core.Extensions;

public static class LinkFilterExtension
{
    public static IEnumerable<string> ApplyFilters(this IEnumerable<string> links, string baseUrl)
    {
        var uri = new Uri(baseUrl);


        return links
            .Distinct()
            .Select(link => new Uri(uri, link))
            .RemoveAnchorLinks()
            .RemoveNotAbsolute()
            .CheckLinksHosts(uri)
            .Select(link => link.AbsoluteUri)
            .CheckForSlashAndRemove();
    }

    private static IEnumerable<string> CheckForSlashAndRemove(this IEnumerable<string> links)
    {
        return links.Select(link => link.EndsWith("/") ? link.Remove(link.Length - 1) : link);
    }

    private static IEnumerable<Uri> RemoveNotAbsolute(this IEnumerable<Uri> links)
    {
        return links.Where(link => link.IsAbsoluteUri);
    }

    private static IEnumerable<Uri> CheckLinksHosts(this IEnumerable<Uri> links, Uri uri)
    {
        return links.Where(link => link.Host == uri.Host);
    }

    private static IEnumerable<Uri> RemoveAnchorLinks(this IEnumerable<Uri> links)
    {
        return links.Where(link => !link.AbsolutePath.Contains('#'));
    }
}