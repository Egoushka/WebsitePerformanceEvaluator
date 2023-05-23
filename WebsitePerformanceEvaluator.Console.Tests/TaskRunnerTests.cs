using Moq;
using WebsitePerformanceEvaluator.Console.Core.Helpers;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Models.Enums;
using WebsitePerformanceEvaluator.Core.Service;
using WebsitePerformanceEvaluator.Crawler.Crawlers;
using Xunit;

namespace WebsitePerformanceEvaluator.Console.Tests;

public class TaskRunnerTests
{
    private readonly Mock<CombinedCrawler> _crawlerMock;
    private readonly Mock<ConsoleWrapper> _consoleWrapperMock;
    private readonly Mock<ConsoleHelper> _consoleHelperMock;
    private readonly Mock<LinkService> _linkServiceMock;

    private readonly TaskRunner _taskRunner;

    public TaskRunnerTests()
    {
        _consoleWrapperMock = new Mock<ConsoleWrapper>();
        _consoleHelperMock = new Mock<ConsoleHelper>();
        _linkServiceMock = new Mock<LinkService>(null, null);

        _taskRunner = new TaskRunner(_consoleWrapperMock.Object, _consoleHelperMock.Object, _linkServiceMock.Object);
    }

    [Fact]
    public async Task Run_ValidUrl_CheckIsEnterWebsitePrompted()
    {
        // Arrange
        SetupMocks();

        // Act
        await _taskRunner.RunAsync();

        // Assert
        _consoleWrapperMock.Verify(x => x.WriteLine("Enter website url:"), Times.Once());
        _consoleWrapperMock.Verify(x => x.ReadLine(), Times.Once());
    }
    

    [Fact]
    public async Task Run_ValidUrl_CheckIsPrintedLinksCountBySourceType()
    {
        // Arrange
        SetupMocks();

        // Act
        await _taskRunner.RunAsync();

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
        await _taskRunner.RunAsync();

        // Assert
        _consoleWrapperMock.Verify(x => x.WriteLine("Links with time response:"), Times.Once());
    }
    [Fact]
    public async Task Run_ValidUrl_CheckIsPrintedMessageAboutLinksInSitemap()
    {
        // Arrange
        SetupMocks();

        // Act
        await _taskRunner.RunAsync();

        // Assert
        _consoleWrapperMock.Verify(x => x.WriteLine("Links in sitemap, that wasn't found after crawling:"), Times.Once());
    }
    [Fact]
    public async Task Run_ValidUrl_CheckIsPrintedMessageAboutLinksInWebsite()
    {
        // Arrange
        SetupMocks();

        // Act
        await _taskRunner.RunAsync();

        // Assert
        _consoleWrapperMock.Verify(x => x.WriteLine("Links found after crawling website, but not in sitemap:"), Times.Once());
    }

    [Fact]
    public async Task Run_ValidUrl_CheckIsTableWithLinksInSitemapPrinted()
    {
        // Arrange
        var url = "https://ukad-group.com/";
        
        var expectedLinkPerformances = new List<LinkPerformance>
        {
            new()
            {
                CrawlingLinkSource = CrawlingLinkSource.Sitemap,
                Url = "https://ukad-group.com/contacts"
            }
        };
        
        var expectedLinksInString = new List<string>
        {
            "https://ukad-group.com/contacts"
        };
        
        SetupMocks(url, expectedLinkPerformances);

        // Act
        await _taskRunner.RunAsync();

        // Assert
        _consoleHelperMock.Verify(x => x.PrintTable(It.IsAny<IEnumerable<string>>(), expectedLinksInString), Times.Once);
    }

    [Fact]
    public async Task Run_ValidUrl_CheckIsTableWithLinksInWebsitePrinted()
    {
        // Arrange
        var url = "https://ukad-group.com/";
        
        var expectedLinkPerformances = new List<LinkPerformance>
        {
            new()
            {
                CrawlingLinkSource = CrawlingLinkSource.Website,
                Url = "https://ukad-group.com/contacts"
            }
        };
        
        var expectedLinksInString = new List<string>
        {
            "https://ukad-group.com/contacts"
        };
        
        SetupMocks(url, expectedLinkPerformances);

        // Act
        await _taskRunner.RunAsync();

        // Assert
        _consoleHelperMock.Verify(x => x.PrintTable(It.IsAny<IEnumerable<string>>(), expectedLinksInString), Times.Once);
    }
    
    [Fact]
    public async Task Run_ValidUrl_CheckIsTableWithTimeResponsesPrintedInRightOrder()
    {
        // Arrange
        var url = "https://ukad-group.com/";
        
        var expectedLinkPerformances = new List<LinkPerformance>
        {
            new()
            {
                CrawlingLinkSource = CrawlingLinkSource.Sitemap,
                Url = "https://ukad-group.com/2",
                TimeResponseMs = 200
            },
            new()
            {
                CrawlingLinkSource = CrawlingLinkSource.Website,
                Url = "https://ukad-group.com/1",
                TimeResponseMs = 100
            }
        };
        
        var expectedLinksWithTimeResponse = new List<Tuple<string, long?>>
        {
            new("https://ukad-group.com/1", 100),
            new("https://ukad-group.com/2", 200),
        };
        
        SetupMocks(url, expectedLinkPerformances);

        // Act
        await _taskRunner.RunAsync();

        // Assert
        _consoleHelperMock.Verify(x => x.PrintTable(It.IsAny<IEnumerable<string>>(), expectedLinksWithTimeResponse), Times.Once);
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
            .Setup(x => x.FindLinksAsync(url))
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
                Url = "https://ukad-group.com/1",
                CrawlingLinkSource = CrawlingLinkSource.Website,
                TimeResponseMs = 100
            },
            new()
            {
                Url = "https://ukad-group.com/2",
                CrawlingLinkSource = CrawlingLinkSource.Sitemap,
                TimeResponseMs = 100
            },
            new()
            {
                Url = "https://ukad-group.com/3",
                CrawlingLinkSource = CrawlingLinkSource.WebsiteAndSitemap,
                TimeResponseMs = 100
            }
        };

        return links;
    }
}