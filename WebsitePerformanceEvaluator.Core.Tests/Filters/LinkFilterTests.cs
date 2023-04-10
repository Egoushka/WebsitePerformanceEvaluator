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
    public void FilterLinks_WhenGivenEmptyList_ShouldReturnEmptyList()
    {
        // Arrange
        var baseUrl = "https://example.com/";

        _validatorMock
            .Setup(x => x.IsValidLink(It.IsAny<string>()))
            .Returns(true);

        // Act
        var result = _linkFilter.FilterLinks(new List<LinkPerformance>(), baseUrl);

        // Assert
        Assert.Empty(result);
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
    public void FilterLinks_WhenGivenLinks_ShouldReturnFilteredList()
    {
        // Arrange
        var baseUrl = "https://example.com/";

        var links = new List<LinkPerformance>
        {
            new() { Link = "https://example.com/" },
            new() { Link = "https://example.com/about-us" },
            new() { Link = "https://example.com/blog" },
            new() { Link = "https://example.com/contact" },
            new() { Link = "https://external.com" },
            new() { Link = "https://example.com/#anchor" }
        };

        _validatorMock
            .Setup(x => x.IsValidLink(It.IsNotIn("https://example.com/#anchor")))
            .Returns(true);

        // Act
        var result = _linkFilter.FilterLinks(links, baseUrl);

        // Assert
        Assert.Equal(4, result.Count());
        Assert.True(result.All(link => link.Link.StartsWith("https://example.com")));
        Assert.DoesNotContain(result, link => link.Link.Contains("#anchor"));
        Assert.DoesNotContain(result, link => link.Link.StartsWith("http://"));
        Assert.DoesNotContain(result, link => link.Link.StartsWith("https://external.com"));
    }
}