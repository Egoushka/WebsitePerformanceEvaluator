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

    [Theory]
    [InlineData("https://example.com/", "/page", "https://example.com/page")]
    [InlineData("https://example.com", "/page", "https://example.com/page")]
    [InlineData("https://example.com", "page", "page")]
    public void AddBaseUrl_ShouldAddBaseUrlToLinks(string baseUrl, string link, string expected)
    {
        // Arrange
        var links = new List<LinkPerformance>
        {
            new() { Link = link }
        };

        // Act
        var result = _linkHelper.AddBaseUrl(links, baseUrl).ToList();

        // Assert
        Assert.Single(result);
        Assert.Equal(expected, result[0].Link);
    }

    [Theory]
    [InlineData("https://example.com")]
    public void RemoveLastSlashFromLinks_ShouldRemoveLastSlashFromLinks(string baseUrl)
    {
        // Arrange
        var links = new List<LinkPerformance>
        {
            new() { Link = baseUrl },
            new() { Link = baseUrl + "/page/" },
            new() { Link = baseUrl + "/contact" }
        };

        // Act
        var result = _linkHelper.RemoveLastSlashFromLinks(links).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.Equal(baseUrl, result[0].Link);
        Assert.Equal(baseUrl + "/page", result[1].Link);
        Assert.Equal(baseUrl + "/contact", result[2].Link);
    }

    [Fact]
    public async Task AddResponseTimeAsync_ShouldAddResponseTimeToLinks()
    {
        // Arrange
        var expectedTimes = new[] { 100, 200, 300 };
        var links = new List<LinkPerformance>
        {
            new() { Link = "https://example.com" },
            new() { Link = "https://example.com/page" },
            new() { Link = "https://example.com/contact" }
        };
        _httpClientServiceMock.Setup(x => x.GetTimeResponseAsync(It.IsAny<string>())).ReturnsAsync((string url) =>
        {
            return url switch
            {
                "https://example.com" => 100L,
                "https://example.com/page" => 200L,
                _ => url == "https://example.com/contact" ? 300L : 0L
            };
        });

        // Act
        var result = (await _linkHelper.AddResponseTimeAsync(links)).ToList();

        // Assert
        Assert.Equal(3, result.Count);
        Assert.All(result, link => Assert.Equal(expectedTimes[result.IndexOf(link)], link.TimeResponseMs));
    }
}