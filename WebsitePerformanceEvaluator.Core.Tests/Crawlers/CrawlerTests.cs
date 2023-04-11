using AutoFixture;
using Moq;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;
using Xunit;

namespace WebsitePerformanceEvaluator.Core.Tests.Crawlers;

public class CrawlerTests
{
    private readonly Crawler _crawler;
    private readonly Fixture _fixture;
    private readonly Mock<SitemapCrawler> _sitemapCrawlerMock;
    private readonly Mock<WebsiteCrawler> _websiteCrawlerMock;

    public CrawlerTests()
    {
        _websiteCrawlerMock = new Mock<WebsiteCrawler>();
        _sitemapCrawlerMock = new Mock<SitemapCrawler>();
        _crawler = new Crawler(_websiteCrawlerMock.Object, _sitemapCrawlerMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task CrawlWebsiteAndSitemapAsync_WhenNoneReturnEmptyList_ReturnsAllLinksAndCheckThem()
    {
        // Arrange
        var expectedWebsiteLinks = GetExpectedLinks(CrawlingLinkSource.Website, 3);
        var expectedSitemapLinks = GetExpectedLinks(CrawlingLinkSource.Sitemap, 3);

        _websiteCrawlerMock.Setup(x => x.FindLinksAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedWebsiteLinks);
        _sitemapCrawlerMock.Setup(x => x.FindLinksAsync(It.IsAny<string>()))
            .ReturnsAsync(expectedSitemapLinks);

        // Act
        var result = await _crawler.CrawlWebsiteAndSitemapAsync(_fixture.Create<string>());

        // Assert
        var expectedLinks = expectedWebsiteLinks.Union(expectedSitemapLinks);

        Assert.Equal(expectedLinks.Count(), result.Count());

        var websiteAndSitemapLinks = result.Where(x => x.CrawlingLinkSource == CrawlingLinkSource.WebsiteAndSitemap);
        Assert.Equal(expectedWebsiteLinks.Intersect(expectedSitemapLinks).Count(), websiteAndSitemapLinks.Count());

        var websiteLinks = result.Where(x => x.CrawlingLinkSource == CrawlingLinkSource.Website);
        Assert.Equal(expectedWebsiteLinks.Except(expectedSitemapLinks).Count(), websiteLinks.Count());

        var sitemapLinks = result.Where(x => x.CrawlingLinkSource == CrawlingLinkSource.Sitemap);
        Assert.Equal(expectedSitemapLinks.Except(expectedWebsiteLinks).Count(), sitemapLinks.Count());
    }

    [Fact]
    public async Task CrawlWebsiteAndSitemapAsync_WhenWebsiteCrawlerReturnsEmptyList_ReturnsSitemapLinksOnly()
    {
        // Arrange
        var websiteUrl = _fixture.Create<string>();
        var expectedSitemapLinks = GetExpectedLinks(CrawlingLinkSource.Sitemap, 3);

        _websiteCrawlerMock.Setup(x => x.FindLinksAsync(websiteUrl)).ReturnsAsync(new List<LinkPerformance>());
        _sitemapCrawlerMock.Setup(x => x.FindLinksAsync(websiteUrl)).ReturnsAsync(expectedSitemapLinks);

        // Act
        var result = await _crawler.CrawlWebsiteAndSitemapAsync(websiteUrl);

        // Assert
        Assert.Equal(expectedSitemapLinks, result);
    }

    [Fact]
    public async Task CrawlWebsiteAndSitemapAsync_WhenSitemapCrawlerReturnsEmptyList_ReturnsWebsiteLinksOnly()
    {
        // Arrange
        var websiteUrl = _fixture.Create<string>();
        var expectedWebsiteLinks = GetExpectedLinks(CrawlingLinkSource.Website, 3);

        _websiteCrawlerMock.Setup(x => x.FindLinksAsync(websiteUrl)).ReturnsAsync(expectedWebsiteLinks);
        _sitemapCrawlerMock.Setup(x => x.FindLinksAsync(websiteUrl)).ReturnsAsync(new List<LinkPerformance>());

        // Act
        var result = await _crawler.CrawlWebsiteAndSitemapAsync(websiteUrl);

        // Assert
        Assert.Equal(expectedWebsiteLinks, result);
    }

    [Fact]
    public async Task CrawlWebsiteAndSitemapAsync_WhenCalled_CallsFindLinksAsyncWithCorrectUrl()
    {
        // Arrange
        var url = _fixture.Create<string>();

        // Act
        await _crawler.CrawlWebsiteAndSitemapAsync(url);

        // Assert
        _websiteCrawlerMock.Verify(x => x.FindLinksAsync(url), Times.Once);
        _sitemapCrawlerMock.Verify(x => x.FindLinksAsync(url), Times.Once);
    }

    [Fact]
    public async Task CrawlWebsiteAndSitemapAsync_WhenCrawlersReturnEmptyLists_ReturnsEmptyList()
    {
        // Arrange
        var url = _fixture.Create<string>();

        _websiteCrawlerMock.Setup(x => x.FindLinksAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<LinkPerformance>());

        _sitemapCrawlerMock.Setup(x => x.FindLinksAsync(It.IsAny<string>()))
            .ReturnsAsync(new List<LinkPerformance>());

        // Act
        var result = await _crawler.CrawlWebsiteAndSitemapAsync(url);

        // Assert
        Assert.Empty(result);
    }

    private IEnumerable<LinkPerformance> GetExpectedLinks(CrawlingLinkSource source, int count)
    {
        return _fixture.Build<LinkPerformance>()
            .With(link => link.CrawlingLinkSource, source)
            .CreateMany(count);
    }
}