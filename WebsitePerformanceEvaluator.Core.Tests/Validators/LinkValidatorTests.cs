using WebsitePerformanceEvaluator.Crawler.Validators;
using Xunit;

namespace WebsitePerformanceEvaluator.Core.Tests.Validators;

public class LinkValidatorTests
{
    private readonly LinkValidator _linkValidator;

    public LinkValidatorTests()
    {
        _linkValidator = new LinkValidator();
    }

    [Fact]
    public void IsValidLink_WithAnchor_ReturnsFalse()
    {
        // Arrange
        var link = "https://www.example.com/#about";

        // Act
        var result = _linkValidator.IsValidLink(link);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidLink_WithAttributes_ReturnsFalse()
    {
        // Arrange
        var link = "https://www.example.com/?search=123";

        // Act
        var result = _linkValidator.IsValidLink(link);

        // Assert
        Assert.False(result);
    }

    [Fact]
    public void IsValidLink_ValidLink_ReturnsTrue()
    {
        // Arrange
        var link = "https://www.example.com/about";

        // Act
        var result = _linkValidator.IsValidLink(link);

        // Assert
        Assert.True(result);
    }
}