namespace WebsitePerformanceEvaluator.Core.Validators;

public class LinkValidator
{
    public bool IsValidLink(string link)
    {
        return DoesNotContainAnchor(link) 
               && IsNotFileLink(link) 
               && DoesNotContainAttributes(link)
               && IsHttpOrHttpsLink(link);
    }

    private bool DoesNotContainAnchor(string link)
    {
        return !link.Contains('#');
    }

    private bool IsNotFileLink(string link)
    {
        return link.LastIndexOf('.') < link.LastIndexOf('/');
    }

    private bool DoesNotContainAttributes(string link)
    {
        return !link.Contains('?');
    }

    private bool IsHttpOrHttpsLink(string link)
    {
        return link.StartsWith(Uri.UriSchemeHttp) || link.StartsWith(Uri.UriSchemeHttps);
    }
}