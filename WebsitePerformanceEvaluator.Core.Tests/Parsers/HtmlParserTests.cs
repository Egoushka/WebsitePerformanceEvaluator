using AutoFixture;
using HtmlAgilityPack;
using Moq;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;
using WebsitePerformanceEvaluator.Core.Parsers;
using WebsitePerformanceEvaluator.Core.Service;
using Xunit;

namespace WebsitePerformanceEvaluator.Core.Tests.Parsers;


public class HtmlParserTests
{
    private readonly Mock<HttpClientService> _httpClientServiceMock;
    private readonly HtmlParser _htmlParser;
    private readonly Fixture _fixture;

    public HtmlParserTests()
    {
        _httpClientServiceMock = new Mock<HttpClientService>();
        _htmlParser = new HtmlParser(_httpClientServiceMock.Object);
        _fixture = new Fixture();
    }

    [Fact]
    public async Task GetLinksAsync_WhenClientReturnsNull_ShouldReturnListWithSingleLink()
    {
        // Arrange
        var url = "https://example.com";
        _httpClientServiceMock.Setup(service => service.GetDocumentAsync(It.IsAny<LinkPerformance>()))
            .ReturnsAsync(new HtmlDocument());

        // Act
        var result = (await _htmlParser.GetLinksAsync(url)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(url, result[0].Link);
        Assert.Equal(CrawlingLinkSource.Website, result[0].CrawlingLinkSource);
    }

    [Fact]
    public async Task GetLinksAsync_WhenNoLinksAreFound_ShouldReturnListWithSingleLink()
    {
        // Arrange
        var url = _fixture.Create<Uri>().ToString();
        _httpClientServiceMock.Setup(service => service.GetDocumentAsync(It.IsAny<LinkPerformance>()))
            .ReturnsAsync(new HtmlDocument());

        // Act
        var result = (await _htmlParser.GetLinksAsync(url)).ToList();

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(url, result[0].Link);
        Assert.Equal(CrawlingLinkSource.Website, result[0].CrawlingLinkSource);
    }

    [Fact]
    public async Task GetLinksAsync_WhenLinksFound_ShouldReturnList()
    {
        //Arrange
        var url = _fixture.Create<Uri>().ToString();
        
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml("<html><body><a href=\"https://example.com/1\">1</a><a href=\"https://example.com/2\">2</a></body></html>");
        
        _httpClientServiceMock.Setup(service => service.GetDocumentAsync(It.IsAny<LinkPerformance>()))
            .ReturnsAsync(htmlDocument);
        
        //Act
        var result = (await _htmlParser.GetLinksAsync(url)).ToList();
        
        //Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Count);
    }
}