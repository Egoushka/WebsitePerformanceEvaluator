using Moq;
using WebsitePerformanceEvaluator.Core.Crawlers;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;
using Xunit;

namespace WebsitePerformanceEvaluator.Tests;

public class TaskRunnerTests
{
    private readonly TaskRunner _taskRunner;
    private readonly Mock<Crawler> _crawlerMock;
    private readonly Mock<ConsoleWrapper> _consoleWrapperMock;
    private readonly Mock<ConsoleHelper> _consoleHelperMock;

    public TaskRunnerTests()
    {
        _crawlerMock = new Mock<Crawler>(null, null);
        _consoleWrapperMock = new Mock<ConsoleWrapper>();
        _consoleHelperMock = new Mock<ConsoleHelper>();
        _taskRunner = new TaskRunner(_crawlerMock.Object, _consoleWrapperMock.Object, _consoleHelperMock.Object);
    }

    [Fact]
    public async Task Run_ValidUrl_CheckIsEnterWebsitePrompted()
    {
        // Arrange
        SetupMocks();

        // Act
        await _taskRunner.Run();

        // Assert
        _consoleWrapperMock.Verify(x => x.WriteLine("Enter website url:"), Times.Once());
        _consoleWrapperMock.Verify(x => x.ReadLine(), Times.Once());
    }

    [Fact]
    public async Task Run_ValidUrl_IsCalledCrawlingMethod()
    {
        // Arrange
        SetupMocks();

        // Act
        await _taskRunner.Run();

        // Assert
        _consoleWrapperMock.Verify(x => x.WriteLine("Links found after crawling website, but not in sitemap:"),
            Times.Once());
    }

    [Fact]
    public async Task Run_ValidUrl_CheckIsTablesPrintingCalled()
    {
        // Arrange
        SetupMocks();

        // Act
        await _taskRunner.Run();

        // Assert
        _consoleHelperMock.Verify(x => x.PrintTable(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()),
            Times.Exactly(2));
        _consoleHelperMock.Verify(
            x => x.PrintTable(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<Tuple<string, long?>>>()),
            Times.Exactly(1));
    }

    [Fact]
    public async Task Run_ValidUrl_CheckIsPrintedLinksCountBySourceType()
    {
        // Arrange
        SetupMocks();

        // Act
        await _taskRunner.Run();

        // Assert
        _consoleWrapperMock.Verify(x => x.WriteLine("Links in sitemap: 2"), Times.Once());
        _consoleWrapperMock.Verify(x => x.WriteLine("Links after crawling: 2"), Times.Once());
    }

    [Fact]
    public async Task Run_ValidUrl_CheckIsPrintedMessageAboutLinksWithTimeResponse()
    {
        // Arrange
        SetupMocks();

        // Act
        await _taskRunner.Run();

        // Assert
        _consoleWrapperMock.Verify(x => x.WriteLine("Links with time response:"), Times.Once());
    }

    [Fact]
    public async Task Run_ValidUrl_CheckIsPrintedMessageAboutLinksInSitemap()
    {
        // Arrange
        var url = "https://ukad-group.com/";
        var expectedLinkPerformances = new List<LinkPerformance>
        {
            new()
            {
                CrawlingLinkSource = CrawlingLinkSource.Sitemap,
                Link = "https://ukad-group.com/contacts"
            }
        };
        SetupMocks(url, expectedLinkPerformances);

        // Act
        await _taskRunner.Run();

        // Assert
        _consoleWrapperMock.Verify(x => x.WriteLine("Links found after crawling website, but not in sitemap:"),
            Times.Once());
        _consoleWrapperMock.Verify(x => x.WriteLine("No links found"), Times.Once());
        _consoleWrapperMock.Verify(x => x.WriteLine("Links in sitemap, that wasn't found after crawling:"),
            Times.Once());
    }

    [Fact]
    public async Task Run_ValidUrl_CheckIsPrintedMessageAboutLinksInWebsite()
    {
        // Arrange
        var url = "https://ukad-group.com/";
        var expectedLinkPerformances = new List<LinkPerformance>
        {
            new()
            {
                CrawlingLinkSource = CrawlingLinkSource.Website,
                Link = "https://ukad-group.com/contacts"
            }
        };
        SetupMocks(url, expectedLinkPerformances);

        // Act
        await _taskRunner.Run();

        // Assert
        _consoleWrapperMock.Verify(x => x.WriteLine("Links found after crawling website, but not in sitemap:"),
            Times.Once());
        _consoleWrapperMock.Verify(x => x.WriteLine("Links in sitemap, that wasn't found after crawling:"),
            Times.Once());
        _consoleWrapperMock.Verify(x => x.WriteLine("No links found"), Times.Once());

    }

    private void SetupMocks()
    {
        var url = "https://ukad-group.com/";
        var expectedLinkPerformances = GetDefaultExpectedLinks();

        SetupMocks(url, expectedLinkPerformances);
    }

    private void SetupMocks(string url, IEnumerable<LinkPerformance> expectedLinkPerformances)
    {
        _crawlerMock
            .Setup(x => x.CrawlWebsiteAndSitemapAsync(url))
            .ReturnsAsync(expectedLinkPerformances);
        _consoleWrapperMock
            .Setup(x => x.ReadLine())
            .Returns(url);
        _consoleHelperMock
            .Setup(x => x.PrintTable(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<string>>()));
        _consoleHelperMock
            .Setup(x => x.PrintTable(It.IsAny<IEnumerable<string>>(), It.IsAny<IEnumerable<Tuple<string, long?>>>()));
    }

    private IEnumerable<LinkPerformance> GetDefaultExpectedLinks()
    {
        var links = new List<LinkPerformance>
        {
            new()
            {
                Link = "https://ukad-group.com/1",
                CrawlingLinkSource = CrawlingLinkSource.Website,
                TimeResponseMs = 100
            },
            new()
            {
                Link = "https://ukad-group.com/2",
                CrawlingLinkSource = CrawlingLinkSource.Sitemap,
                TimeResponseMs = 100
            },
            new()
            {
                Link = "https://ukad-group.com/3",
                CrawlingLinkSource = CrawlingLinkSource.WebsiteAndSitemap,
                TimeResponseMs = 100
            }
        };

        return links;
    }
}