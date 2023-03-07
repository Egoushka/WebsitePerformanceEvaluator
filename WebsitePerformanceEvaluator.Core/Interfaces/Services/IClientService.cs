using System.Xml;

namespace WebsitePerformanceEvaluator.Core.Interfaces.Services;

public interface IClientService
{
    public Task<XmlDocument> GetSitemapAsync(string baseUrl);
}