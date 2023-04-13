using Moq;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Helpers;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;
using WebsitePerformanceEvaluator.Core.Parsers;
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
        var linksWithResponseTime = GetExpectedLinks(CrawlingLinkSource.Website, 2);
        var linksWithoutResponseTime = GetExpectedLinksWithoutTimeResponse(CrawlingLinkSource.Website, 2);

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
    public async Task FindLinksAsync_OnMultipleCrawlCalls_ReturnsExpectedLinkCount()
    {
        // Arrange
        var url = "https://example.com";

        var links = new List<LinkPerformance>
        {
            new()
            {
                Link = "https://example.com",
                TimeResponseMs = 100,
            },
            new()
            {
                Link = "https://example.com/1",
                TimeResponseMs = 100,
            },
        };
        var linksAfterSecondCall = new List<LinkPerformance>
        {
            new()
            {
                Link = "https://example.com/1",
                TimeResponseMs = 100,
            },
            new()
            {
                Link = "https://example.com/2",
                TimeResponseMs = 100,
            },
        };

        _htmlParserMock.Setup(x => x.GetLinksAsync("https://example.com"))
            .ReturnsAsync(links);
        _htmlParserMock.Setup(x => x.GetLinksAsync("https://example.com/1"))
            .ReturnsAsync(linksAfterSecondCall);
        
        _linkFilterMock
            .Setup(x => x.FilterLinks(links, url))
            .Returns(links);
        _linkFilterMock
            .Setup(x => x.FilterLinks(linksAfterSecondCall, url))
            .Returns(linksAfterSecondCall);

        _linkHelperMock
            .Setup(x => x.RemoveLastSlashFromLinks(links))
            .Returns(links);
        _linkHelperMock
            .Setup(x => x.RemoveLastSlashFromLinks(linksAfterSecondCall))
            .Returns(linksAfterSecondCall);

        _linkHelperMock
            .Setup(x => x.AddBaseUrl(links, url))
            .Returns(links);
        _linkHelperMock
            .Setup(x => x.AddBaseUrl(linksAfterSecondCall, url))
            .Returns(linksAfterSecondCall);

        // Act
        var result = await _crawler.FindLinksAsync(url);

        // Assert
        Assert.Equal(3, result.Count());
    }
    
    private IEnumerable<LinkPerformance> GetExpectedLinks(CrawlingLinkSource source, int count)
    {
        var links = new List<LinkPerformance>();
        for (var i = 0; i < count; i++)
        {
            links.Add(new LinkPerformance
            {
                CrawlingLinkSource = source,
                Link = $"https://example.com/{i}",
                TimeResponseMs = 100
            });
        }

        return links;
    }

    private IEnumerable<LinkPerformance> GetExpectedLinksWithoutTimeResponse(CrawlingLinkSource source, int count)
    {
        var response = new List<LinkPerformance>();
        for (var i = 0; i < count; i++)
        {
            response.Add(new LinkPerformance
            {
                CrawlingLinkSource = source,
                Link = $"https://example.com/{i}"
            });
        }

        return response;
    }
}