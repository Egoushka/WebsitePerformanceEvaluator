namespace WebsitePerformanceEvaluator.API.Core.Validators;

public class UrlValidator
{
    public bool Validate(string url)
    {
        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) 
               && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}