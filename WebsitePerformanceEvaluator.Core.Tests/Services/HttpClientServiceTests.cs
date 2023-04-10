using System.Net;
using System.Text;
using HtmlAgilityPack;
using Moq;
using Moq.Protected;
using WebsitePerformanceEvaluator.Core.Interfaces;
using WebsitePerformanceEvaluator.Core.Models;
using WebsitePerformanceEvaluator.Core.Service;
using Xunit;

namespace WebsitePerformanceEvaluator.Core.Tests.Services
{
    public class HttpClientServiceTests
    {
        private readonly HttpClientService _httpClientService;
        private readonly Mock<ILogger> _loggerMock;
        private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;

        public HttpClientServiceTests()
        {
            _httpMessageHandlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
            
            var httpClientFactoryMock = new Mock<IHttpClientFactory>();
            var httpClientMock = new Mock<HttpClient>(_httpMessageHandlerMock.Object);
            
            httpClientMock.Object.DefaultRequestHeaders.Clear();
            httpClientMock.Object.DefaultRequestHeaders.Add("User-Agent", "HttpClientServiceTests");
            httpClientFactoryMock.Setup(_ => _.CreateClient(It.IsAny<string>())).Returns(httpClientMock.Object);
            
            _loggerMock = new Mock<ILogger>();
            _httpClientService = new HttpClientService(httpClientFactoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task GetDocumentAsync_ShouldReturnHtmlDocument_WhenGivenValidLink()
        {
            // Arrange
            var expectedHtml = "<html><head></head><body><h1>Hello World!</h1></body></html>";
            var linkPerformance = new LinkPerformance { Link = "https://example.com" };
            
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedHtml, Encoding.UTF8, "text/html")
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);
            
            var expectedDoc = new HtmlDocument();
            expectedDoc.LoadHtml(expectedHtml);

            // Act
            var result = await _httpClientService.GetDocumentAsync(linkPerformance);

            // Assert
            Assert.Equal(expectedDoc.DocumentNode.OuterHtml, result.DocumentNode.OuterHtml);
        }
        
        [Fact]
        public async Task GetDocumentAsync_ShouldReturnEmptyHtmlDocument_WhenMediaTypeIsNotHtml()
        {
            // Arrange
            var expectedHtml = "";
            var linkPerformance = new LinkPerformance { Link = "https://example.com" };
            
            var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(expectedHtml, Encoding.UTF8, "text/plain")
            };

            _httpMessageHandlerMock.Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(httpResponse);
            
            var expectedDoc = new HtmlDocument();
            expectedDoc.LoadHtml(expectedHtml);

            // Act
            var result = await _httpClientService.GetDocumentAsync(linkPerformance);

            // Assert
            Assert.Empty(result.DocumentNode.ChildNodes);
        }
    }
}