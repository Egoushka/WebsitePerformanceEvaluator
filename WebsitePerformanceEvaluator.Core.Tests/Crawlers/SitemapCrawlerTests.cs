using System.Xml;
using Moq;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Helpers;
using WebsitePerformanceEvaluator.Core.Interfaces;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Parsers;
using WebsitePerformanceEvaluator.Core.Service;
using Xunit;

namespace WebsitePerformanceEvaluator.Core.Tests.Crawlers;

public class SitemapCrawlerTests
{
    private readonly SitemapCrawler _crawler;
    private readonly Mock<HttpClientService> _httpClientServiceMock;
    private readonly Mock<LinkFilter> _linkFilterMock;
    private readonly Mock<LinkHelper> _linkHelperMock;
    private readonly Mock<ILogger> _loggerMock;
    private readonly Mock<XmlParser> _xmlParserMock;

    public SitemapCrawlerTests()
    {
        _loggerMock = new Mock<ILogger>();
        _httpClientServiceMock = new Mock<HttpClientService>();
        _xmlParserMock = new Mock<XmlParser>();
        _linkFilterMock = new Mock<LinkFilter>();
        _linkHelperMock = new Mock<LinkHelper>();

        _crawler = new SitemapCrawler(
            _loggerMock.Object,
            _httpClientServiceMock.Object,
            _xmlParserMock.Object,
            _linkFilterMock.Object,
            _linkHelperMock.Object
        );
    }

    [Fact]
    public async Task FindLinksAsync_WhenNeedToFilter_ReturnsFilteredLinks()
    {
        // Arrange
        var baseUrl = "http://example.com";

        var links = new List<LinkPerformance>
        {
            new() { Link = $"{baseUrl}/page1" },
            new() { Link = $"{baseUrl}/page2" }
        };
        var expectedLinks = new List<LinkPerformance>
        {
            new() { Link = $"{baseUrl}/page1" }
        };

        _httpClientServiceMock
            .Setup(x => x.DownloadFileAsync(It.IsAny<string>()))
            .ReturnsAsync(string.Empty);
        _xmlParserMock
            .Setup(x => x.GetLinks(It.IsAny<XmlNodeList>()))
            .Returns(links);
        _linkFilterMock
            .Setup(x => x.FilterLinks(It.IsAny<IEnumerable<LinkPerformance>>(), It.IsAny<string>()))
            .Returns(expectedLinks);
        _linkHelperMock
            .Setup(x => x.RemoveLastSlashFromLinks(It.IsAny<IEnumerable<LinkPerformance>>()))
            .Returns(expectedLinks);
        _linkHelperMock
            .Setup(x => x.AddResponseTimeAsync(It.IsAny<IEnumerable<LinkPerformance>>()))
            .ReturnsAsync(expectedLinks);

        // Act
        var result = await _crawler.FindLinksAsync(baseUrl + "/sitemap.xml");

        // Assert
        Assert.Equal(expectedLinks, result);
    }

    [Fact]
    public async Task FindLinksAsync_WhenSitemapContainsUrls_ShouldCheckCalledMethods()
    {
        // Arrange
        var sitemapUrl = "http://example.com/sitemap.xml";
        var links = new List<LinkPerformance>
        {
            new() { Link = "http://example.com/" },
            new() { Link = "http://example.com/about" }
        };
        var filteredLinks = new List<LinkPerformance>
        {
            new() { Link = "http://example.com/" }
        };

        _httpClientServiceMock
            .Setup(x => x.DownloadFileAsync(It.IsAny<string>()))
            .ReturnsAsync(string.Empty);
        _xmlParserMock
            .Setup(x => x.GetLinks(It.IsAny<XmlNodeList>()))
            .Returns(links);
        _linkFilterMock
            .Setup(x => x.FilterLinks(It.IsAny<IEnumerable<LinkPerformance>>(), It.IsAny<string>()))
            .Returns(filteredLinks);
        _linkHelperMock
            .Setup(x => x.RemoveLastSlashFromLinks(It.IsAny<IEnumerable<LinkPerformance>>()))
            .Returns(filteredLinks);
        _linkHelperMock
            .Setup(x => x.AddResponseTimeAsync(It.IsAny<IEnumerable<LinkPerformance>>()))
            .ReturnsAsync(filteredLinks);

        // Act
        var result = await _crawler.FindLinksAsync(sitemapUrl);

        // Assert
        Assert.Equal(filteredLinks, result);
    }
    [Fact]
    public async Task FindLinksAsync_WhenSitemapNotAvailable_ShouldReturnEmptyList()
    {
        // Arrange
        var sitemapUrl = "http://example.com/sitemap.xml";

        _httpClientServiceMock
            .Setup(x => x.DownloadFileAsync(It.IsAny<string>()))
            .ReturnsAsync(string.Empty);

        // Act
        var result = await _crawler.FindLinksAsync(sitemapUrl);

        // Assert
        Assert.Empty(result);
    }
}