using Moq;
using WebsitePerformanceEvaluator.Core.Helpers;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Service;
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
            new() { Link = baseUrl + "/page/" },
        };

        // Act
        var result = _linkHelper.RemoveLastSlashFromLinks(links).ToList();

        // Assert
        Assert.Equal(baseUrl + "/page", result.FirstOrDefault()?.Link);
    }
    [Fact]
    public void RemoveLastSlashFromLinks_WhenGivenLinkWithoutSlashAtTheEnd_ShouldReturnAsItWas()
    {
        // Arrange
        var baseUrl = "https://example.com";
        
        var links = new List<LinkPerformance>
        {
            new() { Link = baseUrl + "/page" },
        };

        // Act
        var result = _linkHelper.RemoveLastSlashFromLinks(links).ToList();

        // Assert
        Assert.Equal(baseUrl + "/page", result.FirstOrDefault()?.Link);
    }

    [Fact]
    public async Task AddResponseTimeAsync_WithValidLinks_ShouldSetResponseTimeForEachLink()
    {
        // Arrange
        var expectedTimes = new[] { 100, 200, 300 };
        var links = new List<LinkPerformance>
        {
            new() { Link = "https://example.com" },
            new() { Link = "https://example.com/page" },
            new() { Link = "https://example.com/contact" }
        };
        
        _httpClientServiceMock
            .Setup(x => x.GetTimeResponseAsync("https://example.com"))
            .ReturnsAsync(100L);
        _httpClientServiceMock
            .Setup(x => x.GetTimeResponseAsync(It.Is<string>(x => x == "https://example.com/page")))
            .ReturnsAsync(200L);
        _httpClientServiceMock
            .Setup(x => x.GetTimeResponseAsync("https://example.com/contact"))
            .ReturnsAsync(300L);

        // Act
        var result = (await _linkHelper.AddResponseTimeAsync(links)).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.All(result, link => Assert.Equal(expectedTimes[result.IndexOf(link)], link.TimeResponseMs));
    }
}