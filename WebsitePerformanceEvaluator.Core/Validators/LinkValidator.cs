namespace WebsitePerformanceEvaluator.Core.Validators;

public class LinkValidator
{
    public bool IsValidLink(string link)
    {
        var doesNotContainAnchor = !link.Contains('#');
        var doesNotContainAttributes = !link.Contains('?');
        
        return doesNotContainAnchor && doesNotContainAttributes;
    }
}