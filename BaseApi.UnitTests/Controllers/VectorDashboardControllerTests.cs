using BaseApi.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Moq.Protected;
using System.Net;

namespace BaseApi.UnitTests.Controllers;

public class VectorDashboardControllerTests
{
    private readonly Mock<IHttpClientFactory> _httpClientFactoryMock;
    private readonly VectorDashboardController _controller;

    public VectorDashboardControllerTests()
    {
        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _controller = new VectorDashboardController(_httpClientFactoryMock.Object);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.Scheme = "https";
        httpContext.Request.Host = new HostString("localhost", 5001);
        _controller.ControllerContext = new ControllerContext()
        {
            HttpContext = httpContext
        };
    }

    [Fact]
    public async Task Get_ReturnsHtml_WithMetricsContent()
    {
        // Arrange
        var mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        mockHttpMessageHandler.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("fake_metrics_data")
            });

        var client = new HttpClient(mockHttpMessageHandler.Object);
        _httpClientFactoryMock.Setup(f => f.CreateClient(It.IsAny<string>())).Returns(client);

        // Act
        var result = await _controller.Get() as ContentResult;

        // Assert
        result.Should().NotBeNull();
        result!.ContentType.Should().Be("text/html");
        result.Content.Should().Contain("fake_metrics_data");
    }
}
