using HtmlAgilityPack;
using Moq;
using WebsitePerformanceEvaluator.Core.Parsers;
using WebsitePerformanceEvaluator.Core.Service;
using WebsitePerformanceEvaluator.Domain.Enums;
using WebsitePerformanceEvaluator.Domain.Models;
using Xunit;

namespace WebsitePerformanceEvaluator.Core.Tests.Parsers;

public class HtmlParserTests
{
    private readonly HtmlParser _htmlParser;
    private readonly Mock<HttpClientService> _httpClientServiceMock;

    public HtmlParserTests()
    {
        _httpClientServiceMock = new Mock<HttpClientService>();
        _htmlParser = new HtmlParser(_httpClientServiceMock.Object);
    }
    
    [Fact]
    public async Task GetLinksAsync_WhenNoLinksFound_ShouldReturnItself()
    {
        // Arrange
        var url = "https://example.com";
        
        _httpClientServiceMock.Setup(service => service.GetDocumentAsync(It.IsAny<LinkPerformance>()))
            .ReturnsAsync(new HtmlDocument());

        // Act
        var result = await _htmlParser.GetLinksAsync(url);
        
        // Assert
        Assert.Single(result);
        Assert.Equal(url, result.First().Url);
    }
    
    [Fact]
    public async Task GetLinkAsync_WhenLinksFound_ShouldReturnInputAsFirstItem()
    {
        //Arrange
        var url = "https://example.com";
        
        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(
            "<html><body><a href=\"https://example.com/1\">1</a><a href=\"https://example.com/2\">2</a></body></html>");

        _httpClientServiceMock.Setup(service => service.GetDocumentAsync(It.IsAny<LinkPerformance>()))
            .ReturnsAsync(htmlDocument);

        //Act
        var result = await _htmlParser.GetLinksAsync(url);
        
        //Assert
        Assert.Contains(result, item => item.Url == url);
    }
    
    [Fact]
    public async Task GetLinkAsync_WhenLinksFound_ShouldReturnAllLinksFromHtmlDocument()
    {
        //Arrange
        var url = "https://example.com";
        
        var expectedLinks = new List<string>
        {
            "https://example.com",
            "https://example.com/1",
            "https://example.com/2"
        };

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(
            "<html><body><a href=\"https://example.com/1\">1</a><a href=\"https://example.com/2\">2</a></body></html>");

        _httpClientServiceMock.Setup(service => service.GetDocumentAsync(It.IsAny<LinkPerformance>()))
            .ReturnsAsync(htmlDocument);

        //Act
        var result = await _htmlParser.GetLinksAsync(url);

        //Assert
        Assert.Equal(3, result.Count());
        Assert.All(result, link => Assert.Contains(link.Url, expectedLinks));
    }

    [Fact]
    public async Task GetLinkAsync_WhenLinksFound_AllShouldHaveWebsiteSource()
    {
        //Arrange
        var url = "https://example.com";

        var htmlDocument = new HtmlDocument();
        htmlDocument.LoadHtml(
            "<html><body><a href=\"https://example.com/1\">1</a><a href=\"https://example.com/2\">2</a></body></html>");

        _httpClientServiceMock.Setup(service => service.GetDocumentAsync(It.IsAny<LinkPerformance>()))
            .ReturnsAsync(htmlDocument);

        //Act
        var result = await _htmlParser.GetLinksAsync(url);

        //Assert
        Assert.All(result, link => Assert.Equal(CrawlingLinkSource.Website, link.CrawlingLinkSource));
    }
}