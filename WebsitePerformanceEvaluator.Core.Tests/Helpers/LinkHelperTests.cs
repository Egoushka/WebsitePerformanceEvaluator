using Moq;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Crawler.Helpers;
using WebsitePerformanceEvaluator.Crawler.Services;
using Xunit;

namespace WebsitePerformanceEvaluator.Core.Tests.Helpers;

public class LinkHelperTests
{
    private readonly Mock<HttpClientService> _httpClientServiceMock;
    private readonly LinkHelper _linkHelper;

    public LinkHelperTests()
    {
        _httpClientServiceMock = new Mock<HttpClientService>();
        _linkHelper = new LinkHelper(_httpClientServiceMock.Object);
    }

    [Fact]
    public void RemoveLastSlashFromLinks_WhenGivenLinksWithSlashAtTheEnd_ShouldRemoveLastSlashFromLinks()
    {
        // Arrange
        var baseUrl = "https://example.com";
        
        var links = new List<LinkPerformance>
        {
            new() { Url = baseUrl + "/page/" },
        };

        // Act
        var result = _linkHelper.RemoveLastSlashFromLinks(links).ToList();

        // Assert
        Assert.All(result, link => Assert.False(link.Url.EndsWith("/")));

    }
    
    [Fact]
    public void RemoveLastSlashFromLinks_WhenGivenLinkWithoutSlashAtTheEnd_ShouldReturnAsItWas()
    {
        // Arrange
        var expected = "https://example.com/page";

        var links = new List<LinkPerformance>
        {
            new() { Url = expected },
        };

        // Act
        var result = _linkHelper.RemoveLastSlashFromLinks(links).ToList();

        // Assert
        Assert.All(result, link => Assert.False(link.Url.EndsWith("/")));
    }

    [Fact]
    public async Task AddResponseTimeAsync_WithValidLinks_ShouldSetResponseTimeForEachLink()
    {
        // Arrange
        var expectedTimes = new[] { 100, 200, 300 };
        var links = new List<LinkPerformance>
        {
            new() { Url = "https://example.com/1" },
            new() { Url = "https://example.com/2" },
            new() { Url = "https://example.com/3" }
        };
        
        _httpClientServiceMock
            .Setup(x => x.GetTimeResponseAsync("https://example.com/1"))
            .ReturnsAsync(100L);
        _httpClientServiceMock
            .Setup(x => x.GetTimeResponseAsync("https://example.com/2"))
            .ReturnsAsync(200L);
        _httpClientServiceMock
            .Setup(x => x.GetTimeResponseAsync("https://example.com/3"))
            .ReturnsAsync(300L);

        // Act
        var result = (await _linkHelper.AddResponseTimeAsync(links)).ToList();

        // Assert
        Assert.All(result, link => Assert.Equal(expectedTimes.ElementAt(result.IndexOf(link)), link.TimeResponseMs));
    }
}