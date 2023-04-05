namespace WebsitePerformanceEvaluator.Core.Validators;

public class LinkValidator
{
    public bool IsValidLink(string link)
    {
        var doesNotContainAnchor = !link.Contains('#');
        var isNotFileLink = link.LastIndexOf('.') < link.LastIndexOf('/');
        var doesNotContainAttributes = !link.Contains('?');
        
        return doesNotContainAnchor && isNotFileLink && doesNotContainAttributes;
    }
}