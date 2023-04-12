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
    public async Task FindLinksAsync_WhenAvailable_ShouldReturnLinksWithResponseTime()
    {
        // Arrange
        var url = "https://www.google.com";
        var linksWithResponseTime = CrawlersUtils.GetExpectedLinks(CrawlingLinkSource.Website, 2);
        var linksWithoutResponseTime = CrawlersUtils.GetExpectedLinksWithoutTimeResponse(CrawlingLinkSource.Website, 2);

        var links = linksWithResponseTime.Concat(linksWithoutResponseTime);

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

    [Fact]
    public async Task FindLinksAsync_WhenPassedLink_ShouldCheckIsMethodsCalled()
    {
        // Arrange
        var url = "https://www.google.com";

        var links = CrawlersUtils.GetExpectedLinks(CrawlingLinkSource.Website, 2);

        _htmlParserMock.SetupSequence(x => x.GetLinksAsync(url))
            .ReturnsAsync(links)
            .ReturnsAsync(new List<LinkPerformance>())
            .ReturnsAsync(new List<LinkPerformance>())
            .ReturnsAsync(new List<LinkPerformance>())
            .ReturnsAsync(new List<LinkPerformance>());

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
        await _crawler.FindLinksAsync(url);

        // Assert
        var expectedTimes = Times.Exactly(3);
        
        _htmlParserMock.Verify(x => x.GetLinksAsync(It.IsAny<string>()), 
            expectedTimes);
        _linkFilterMock.Verify(x => x.FilterLinks(It.IsAny<IEnumerable<LinkPerformance>>(), It.IsAny<string>()),
            expectedTimes);
        _linkHelperMock.Verify(x => x.RemoveLastSlashFromLinks(It.IsAny<IEnumerable<LinkPerformance>>()),
            expectedTimes);
        _linkHelperMock.Verify(x => x.AddBaseUrl(It.IsAny<IEnumerable<LinkPerformance>>(), It.IsAny<string>()),
            expectedTimes);
    }
}