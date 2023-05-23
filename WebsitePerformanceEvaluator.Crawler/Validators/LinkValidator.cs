using WebsitePerformanceEvaluator.Core.Interfaces.Validators;

namespace WebsitePerformanceEvaluator.Crawler.Validators;

public class LinkValidator : ILinkValidator
{
    public virtual bool IsValidLink(string link)
    {
        var doesNotContainAnchor = !link.Contains('#');
        var doesNotContainAttributes = !link.Contains('?');
        var doesNotFileLink = !link.EndsWith(".pdf") || !link.EndsWith(".jpg") || !link.EndsWith(".png");

        return doesNotContainAnchor && doesNotContainAttributes && doesNotFileLink;
    }
}