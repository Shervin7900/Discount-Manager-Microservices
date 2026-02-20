using BaseApi.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Moq;

namespace BaseApi.UnitTests.Controllers;

public class HealthControllerTests
{
    private readonly Mock<HealthCheckService> _healthCheckServiceMock;
    private readonly HealthController _controller;

    public HealthControllerTests()
    {
        _healthCheckServiceMock = new Mock<HealthCheckService>();
        _controller = new HealthController(_healthCheckServiceMock.Object);
    }

    [Fact]
    public async Task Get_ReturnsOk_WhenHealthy()
    {
        // Arrange
        var entries = new Dictionary<string, HealthReportEntry>();
        var report = new HealthReport(entries, HealthStatus.Healthy, TimeSpan.FromMilliseconds(100));
        _healthCheckServiceMock.Setup(s => s.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>())).ReturnsAsync(report);

        // Act
        var result = await _controller.Get() as OkObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(200);
    }

    [Fact]
    public async Task Get_Returns503_WhenUnhealthy()
    {
        // Arrange
        var entries = new Dictionary<string, HealthReportEntry>();
        var report = new HealthReport(entries, HealthStatus.Unhealthy, TimeSpan.FromMilliseconds(100));
        _healthCheckServiceMock.Setup(s => s.CheckHealthAsync(It.IsAny<Func<HealthCheckRegistration, bool>>(), It.IsAny<CancellationToken>())).ReturnsAsync(report);

        // Act
        var result = await _controller.Get() as ObjectResult;

        // Assert
        result.Should().NotBeNull();
        result!.StatusCode.Should().Be(503);
    }
}
