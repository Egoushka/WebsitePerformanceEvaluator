using Moq;
using WebsitePerformanceEvaluator.Core.Interfaces.FIlters;
using WebsitePerformanceEvaluator.Core.Interfaces.Validators;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Crawler.Filters;
using WebsitePerformanceEvaluator.Crawler.Validators;
using Xunit;

namespace WebsitePerformanceEvaluator.Core.Tests.Filters;

public class LinkFilterTests
{
    private readonly ILinkFilter _linkFilter;
    private readonly Mock<LinkValidator> _validatorMock;

    public LinkFilterTests()
    {
        _validatorMock = new Mock<LinkValidator>();

        _linkFilter = new LinkFilter(_validatorMock.Object);
    }

    [Fact]
    public void FilterLinks_WhenGivenOnlyValidLinks_ShouldReturnThem()
    {
        // Arrange
        var baseUrl = "https://example.com/";
        var links = new List<LinkPerformance>
        {
            new() { Url = "https://example.com/1" },
            new() { Url = "https://example.com/2" },
            new() { Url = "https://example.com/3" }
        };

        _validatorMock
            .Setup(x => x.IsValidLink(It.IsAny<string>()))
            .Returns(true);

        // Act
        var result = _linkFilter.FilterLinks(links, baseUrl);

        // Assert
        Assert.Equal(links, result);
    }

    [Fact]
    public void FilterLinks_WhenGivenOnlyInvalidLinks_ShouldReturnEmptyList()
    {
        // Arrange
        var baseUrl = "https://example.com/";
        var links = new List<LinkPerformance>
        {
            new() { Url = "ftp://example.com" },
            new() { Url = "mailto:me@example.com" },
            new() { Url = "javascript:alert('hello world')" }
        };

        _validatorMock
            .Setup(x => x.IsValidLink(It.IsAny<string>()))
            .Returns(false);

        // Act
        var result = _linkFilter.FilterLinks(links, baseUrl);

        // Assert
        Assert.Empty(result);
    }

    [Fact]
    public void FilterLinks_WhenGivenOnlyLinksWithAnchor_ShouldReturnEmptyList()
    {
        // Arrange
        var baseUrl = "https://example.com/";

        var links = new List<LinkPerformance>
        {
            new() { Url = "https://example.com/#anchor" }
        };
        
        _validatorMock
            .Setup(x => x.IsValidLink(It.IsAny<string>()))
            .Returns(false);

        // Act
        var result = _linkFilter.FilterLinks(links, baseUrl);

        // Assert
        Assert.Empty(result);
    }
    
    [Fact]
    public void FilterLinks_WhenGivenOnlyExternalLinks_ShouldReturnEmptyList()
    {
        // Arrange
        var baseUrl = "https://example.com/";

        var links = new List<LinkPerformance>
        {
            new() { Url = "https://external.com/" },
        };

        _validatorMock
            .Setup(x => x.IsValidLink(It.IsAny<string>()))
            .Returns(false);

        // Act
        var result = _linkFilter.FilterLinks(links, baseUrl);

        // Assert
        Assert.Empty(result);
    }
}