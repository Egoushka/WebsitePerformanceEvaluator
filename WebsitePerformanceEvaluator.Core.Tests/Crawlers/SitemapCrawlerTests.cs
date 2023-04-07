using System.Net;
using System.Xml;
using AutoFixture;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Helpers;
using WebsitePerformanceEvaluator.Core.Interfaces;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Parsers;
using WebsitePerformanceEvaluator.Core.Service;
using Moq;
using Xunit;

namespace WebsitePerformanceEvaluator.Core.Tests.Crawlers;


public class SitemapCrawlerTests
{
    private readonly Mock<ILogger> _loggerMock;
    private readonly Mock<HttpClientService> _httpClientServiceMock;
    private readonly Mock<XmlParser> _xmlParserMock;
    private readonly Mock<LinkFilter> _linkFilterMock;
    private readonly Mock<LinkHelper> _linkHelperMock;
    private readonly SitemapCrawler _crawler;
    private readonly Fixture _fixture;

    public SitemapCrawlerTests()
    {
        _fixture = new Fixture();

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
            _linkHelperMock.Object);
    }


    [Fact]
    public async Task FindLinksAsync_Should_ReturnEmptyList_When_SitemapXmlDoesNotContainUrlElements()
    {
        // Arrange
        var sitemapUrl = _fixture.Create<Uri>().ToString();
        
        _httpClientServiceMock
            .Setup(x => x.DownloadFileAsync(sitemapUrl))
            .ReturnsAsync("<url></url>");
        _xmlParserMock
            .Setup(x => x.GetLinks(It.IsAny<XmlNodeList>()))
            .Returns(new List<LinkPerformance>());
        
        // Act
        var result = await _crawler.FindLinksAsync(sitemapUrl);

        // Assert
        Assert.Empty(result);
    }
    [Fact]
    public async Task FindLinksAsync_Should_ReturnLinks_When_SitemapXmlContainsUrlElements()
    {
        // Arrange
        var links = new List<LinkPerformance>
        {
            new() {Link = "http://example.com/"},
            new() {Link = "http://example.com/about"}
        };

        _httpClientServiceMock
            .Setup(x => x.DownloadFileAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.Create<string>());
        _xmlParserMock
            .Setup(x => x.GetLinks(It.IsAny<XmlNodeList>()))
            .Returns(links);
        _linkFilterMock
            .Setup(x => x.FilterLinks(It.IsAny<IEnumerable<LinkPerformance>>(), It.IsAny<string>()))
            .Returns(links);
        _linkHelperMock
            .Setup(x => x.RemoveLastSlashFromLinks(It.IsAny<IEnumerable<LinkPerformance>>()))
            .Returns(links);
        _linkHelperMock
            .Setup(x => x.AddResponseTimeAsync(It.IsAny<IEnumerable<LinkPerformance>>()))
            .ReturnsAsync(links);

        // Act
        var result = await _crawler.FindLinksAsync(_fixture.Create<Uri>().ToString());

        // Assert
        Assert.Equal(links, result);
    }
    [Fact]
    public async Task FindLinksAsync_Should_ReturnEmptyList_When_SitemapXmlIsEmpty()
    {
        // Arrange
        var sitemapUrl = _fixture.Create<Uri>().ToString();
        
        _httpClientServiceMock
            .Setup(x => x.DownloadFileAsync(sitemapUrl))
            .ReturnsAsync("");
    
        // Act
        var result = await _crawler.FindLinksAsync(sitemapUrl);

        // Assert
        Assert.Empty(result);
    }
    [Fact]
    public async Task FindLinksAsync_Should_ReturnEmptyList_When_SitemapXmlIsMalformed()
    {
        // Arrange
        var sitemapUrl = _fixture.Create<Uri>().ToString();
        
        _httpClientServiceMock
            .Setup(x => x.DownloadFileAsync(sitemapUrl))
            .ReturnsAsync("<invalid-xml");
    
        // Act
        var result = await _crawler.FindLinksAsync(sitemapUrl);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public async Task FindLinksAsync_Should_ReturnFilteredLinks_When_FilteredUrlsExist()
    {
        // Arrange
        var sitemapUrl = _fixture.Create<Uri>().ToString();
        
        var links = new List<LinkPerformance>
        {
            new() {Link = $"{sitemapUrl}/page1"},
            new() {Link = $"{sitemapUrl}/page2"}
        };
        var expectedLinks = new List<LinkPerformance>
        {
            new() {Link = $"{sitemapUrl}/page1"}
        };

        _httpClientServiceMock
            .Setup(x => x.DownloadFileAsync(It.IsAny<string>()))
            .ReturnsAsync(_fixture.Create<string>());
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
        var result = await _crawler.FindLinksAsync(sitemapUrl);

        // Assert
        Assert.Equal(expectedLinks, result);
    }
}
