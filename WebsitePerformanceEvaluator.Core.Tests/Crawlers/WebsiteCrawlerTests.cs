using Moq;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Helpers;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;
using WebsitePerformanceEvaluator.Core.Parsers;
using WebsitePerformanceEvaluator.Core.Tests.Crawlers.Common;
using Xunit;

namespace WebsitePerformanceEvaluator.Core.Tests.Crawlers;

public class WebsiteCrawlerTests
{
    private readonly WebsiteCrawler _crawler;

    private readonly Mock<HtmlParser> _htmlParserMock;
    private readonly Mock<LinkFilter> _linkFilterMock;
    private readonly Mock<LinkHelper> _linkHelperMock;

    public WebsiteCrawlerTests()
    {
        _htmlParserMock = new Mock<HtmlParser>();
        _linkFilterMock = new Mock<LinkFilter>();
        _linkHelperMock = new Mock<LinkHelper>();

        _crawler = new WebsiteCrawler(_htmlParserMock.Object, _linkFilterMock.Object, _linkHelperMock.Object);
    }

    [Fact]
    public async Task FindLinksAsync_WhenNoLinksFound_ShouldReturnEmptyList()
    {
        // Arrange
        var url = "https://www.google.com";
        _htmlParserMock
            .Setup(x => x.GetLinksAsync(url))
            .ReturnsAsync(new List<LinkPerformance>());

        // Act
        var result = await _crawler.FindLinksAsync(url);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindLinksAsync_WhenLinksFound_ShouldReturnLinks()
    {
        // Arrange
        var url = "https://www.google.com";
        var links = CrawlersUtils.GetExpectedLinks(CrawlingLinkSource.Website, 2).ToList();

        _htmlParserMock
            .Setup(x => x.GetLinksAsync(url))
            .ReturnsAsync(links);

        _linkFilterMock
            .Setup(x => x.FilterLinks(links, url))
            .Returns(links);

        _linkHelperMock
            .Setup(x => x.RemoveLastSlashFromLinks(links))
            .Returns(links);

        _linkHelperMock
            .Setup(x => x.AddBaseUrl(links, url))
            .Returns(links);

        // Act
        var result = await _crawler.FindLinksAsync(url);

        // Assert
        Assert.Equal(links, result);
    }

    [Fact]
    public async Task FindLinksAsync_WhenAvailable_ShouldReturnLinksWithResponseTime()
    {
        // Arrange
        var url = "https://www.google.com";
        var links = CrawlersUtils.GetExpectedLinks(CrawlingLinkSource.Website, 2).ToList();
        links[0].TimeResponseMs = 100;
        links[1].TimeResponseMs = null;

        _htmlParserMock
            .Setup(x => x.GetLinksAsync(url))
            .ReturnsAsync(links);

        _linkFilterMock
            .Setup(x => x.FilterLinks(links, url))
            .Returns(links);

        _linkHelperMock
            .Setup(x => x.RemoveLastSlashFromLinks(links))
            .Returns(links);

        _linkHelperMock
            .Setup(x => x.AddBaseUrl(links, url))
            .Returns(links);

        // Act
        var result = await _crawler.FindLinksAsync(url);

        // Assert
        Assert.True(result.All(x => x.TimeResponseMs.HasValue));
    }
}