using Moq;
using WebsitePerformanceEvaluator.Core.Filters;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Validators;
using Xunit;

namespace WebsitePerformanceEvaluator.Core.Tests.Filters;

public class LinkFilterTests
{
    private readonly LinkFilter _linkFilter;
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
            new() { Link = "https://example.com/1" },
            new() { Link = "https://example.com/2" },
            new() { Link = "https://example.com/3" }
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
            new() { Link = "ftp://example.com" },
            new() { Link = "mailto:me@example.com" },
            new() { Link = "javascript:alert('hello world')" }
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
            new() { Link = "https://example.com/#anchor" }
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
            new() { Link = "https://external.com/" },
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