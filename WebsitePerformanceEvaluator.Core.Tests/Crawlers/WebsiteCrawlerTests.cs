using AutoFixture;
using Moq;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Helpers;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Parsers;
using Xunit;

namespace WebsitePerformanceEvaluator.Core.Tests.Crawlers;

public class WebsiteCrawlerTests
{
    private readonly Fixture _fixture;

    private readonly Mock<HtmlParser> _htmlParserMock;
    private readonly Mock<LinkFilter> _linkFilterMock;
    private readonly Mock<LinkHelper> _linkHelperMock;

    private readonly WebsiteCrawler _crawler;

    public WebsiteCrawlerTests()
    {
        _fixture = new Fixture();
        
        _htmlParserMock = new Mock<HtmlParser>();
        _linkFilterMock = new Mock<LinkFilter>();
        _linkHelperMock = new Mock<LinkHelper>();

        _crawler = new WebsiteCrawler(_htmlParserMock.Object, _linkFilterMock.Object, _linkHelperMock.Object);
    }

    [Fact]
    public async Task FindLinksAsync_Should_ReturnEmptyList_When_NoLinksFound()
    {
        // Arrange
        var url = "http://example.com/";
        _htmlParserMock
            .Setup(x => x.GetLinksAsync(url))
            .ReturnsAsync(new List<LinkPerformance>());

        // Act
        var result = await _crawler.FindLinksAsync(url);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindLinksAsync_Should_ReturnLinks_When_LinksFound()
    {
        // Arrange
        var url = _fixture.Create<Uri>().ToString();
        var links = _fixture.CreateMany<LinkPerformance>().ToList();
        
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
    public async Task FindLinksAsync_Should_ReturnLinks_WithResponseTime_When_Available()
    {
        // Arrange
        var url = _fixture.Create<Uri>().ToString();
        var links = _fixture.CreateMany<LinkPerformance>().ToList();
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