using Moq;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;
using WebsitePerformanceEvaluator.Core.Tests.Crawlers.Common;
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
        _sitemapCrawlerMock = new Mock<SitemapCrawler>();
        _crawler = new Crawler(_websiteCrawlerMock.Object, _sitemapCrawlerMock.Object);
    }

    [Fact]
    public async Task CrawlWebsiteAndSitemapAsync_WhenBothWebsiteAndSitemapCrawlersHaveSameLinks_ReturnLinksWithoutCopingThem()
    {
        var url = "https://example.com";
        // Arrange
        var expectedLinks = CrawlersUtils.GetExpectedLinks(CrawlingLinkSource.Website, 3);

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
        var url = "https://example.com";
        // Arrange
        var expectedLinks = CrawlersUtils.GetExpectedLinks(CrawlingLinkSource.Website, 3);

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
        var url = "https://example.com";
        // Arrange
        var expectedLinks = CrawlersUtils.GetExpectedLinks(CrawlingLinkSource.Sitemap, 3);

        _websiteCrawlerMock.Setup(x => x.FindLinksAsync(It.IsAny<string>()))
            .ReturnsAsync(Enumerable.Empty<LinkPerformance>());
        _sitemapCrawlerMock.Setup(x => x.FindLinksAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedLinks);

        // Act
        var result = await _crawler.CrawlWebsiteAndSitemapAsync(url);

        // Assert
        Assert.All(result, x => Assert.Equal(CrawlingLinkSource.Sitemap, x.CrawlingLinkSource));
    }
}