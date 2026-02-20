using FluentAssertions;
using System.Net;

namespace BaseApi.IntegrationTests.Features.Health;

public class HealthCheckEndpointIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public HealthCheckEndpointIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_HealthEndpoint_ReturnsOkAndHealthyStatus()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        // Both /health (legacy middleware) and /api/health (FastEndpoint) should work
        // Testing the FastEndpoint specifically here as it was part of the refactor
        var response = await client.GetAsync("/api/health");

        // Assert
        response.EnsureSuccessStatusCode(); 
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
    }
}
