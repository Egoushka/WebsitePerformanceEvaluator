using Moq;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;
using Xunit;

namespace WebsitePerformanceEvaluator.Core.Tests.Crawlers;

public class CrawlerTests
{
    private readonly Crawler _crawler;
    private readonly Mock<SitemapCrawler> _sitemapCrawlerMock;
    private readonly Mock<WebsiteCrawler> _websiteCrawlerMock;

    public CrawlerTests()
    {
        _websiteCrawlerMock = new Mock<WebsiteCrawler>();
        _sitemapCrawlerMock = new Mock<SitemapCrawler>(null, null, null, null, null);
        _crawler = new Crawler(_websiteCrawlerMock.Object, _sitemapCrawlerMock.Object);
    }

    [Fact]
    public async Task CrawlWebsiteAndSitemapAsync_WhenBothWebsiteAndSitemapCrawlersHaveSameLinks_ReturnLinksWithoutCopingThem()
    {
        // Arrange
        var url = "https://example.com";
        
        var expectedLinks = GetExpectedLinks(CrawlingLinkSource.WebsiteAndSitemap, 3);

        _websiteCrawlerMock.Setup(x => x.FindLinksAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedLinks);
        _sitemapCrawlerMock.Setup(x => x.FindLinksAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedLinks);

        // Act
        var result = await _crawler.CrawlWebsiteAndSitemapAsync(url);

        // Assert
        Assert.Equal(expectedLinks.Count(), result.Count());
        Assert.All(result, x => Assert.Equal(CrawlingLinkSource.WebsiteAndSitemap, x.CrawlingLinkSource));
    }

    [Fact]
    public async Task CrawlWebsiteAndSitemapAsync_WhenCalled_ReturnsExpectedLinksFromWebsiteSource()
    {
        // Arrange
        var url = "https://example.com";
        
        var expectedLinks = GetExpectedLinks(CrawlingLinkSource.Website, 3);

        _websiteCrawlerMock.Setup(x => x.FindLinksAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedLinks);
        _sitemapCrawlerMock.Setup(x => x.FindLinksAsync(It.IsAny<string>()))
            .ReturnsAsync(Enumerable.Empty<LinkPerformance>());

        // Act
        var result = await _crawler.CrawlWebsiteAndSitemapAsync(url);

        // Assert
        Assert.All(result, x => Assert.Equal(CrawlingLinkSource.Website, x.CrawlingLinkSource));
    }

    [Fact]
    public async Task CrawlWebsiteAndSitemapAsync_WhenCalled_ReturnsExpectedLinksFromSitemapSource()
    {
        // Arrange
        var url = "https://example.com";

        var expectedLinks = GetExpectedLinks(CrawlingLinkSource.Sitemap, 3);

        _websiteCrawlerMock.Setup(x => x.FindLinksAsync(It.IsAny<string>()))
            .ReturnsAsync(Enumerable.Empty<LinkPerformance>());
        _sitemapCrawlerMock.Setup(x => x.FindLinksAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedLinks);

        // Act
        var result = await _crawler.CrawlWebsiteAndSitemapAsync(url);

        // Assert
        Assert.All(result, x => Assert.Equal(CrawlingLinkSource.Sitemap, x.CrawlingLinkSource));
    }

    private IEnumerable<LinkPerformance> GetExpectedLinks(CrawlingLinkSource source, int count)
    {
        var links = new List<LinkPerformance>();
        for (var i = 0; i < count; i++)
        {
            links.Add(new LinkPerformance
            {
                CrawlingLinkSource = source,
                Link = $"https://example.com/{i}",
                TimeResponseMs = 100
            });
        }

        return links;
    }
}