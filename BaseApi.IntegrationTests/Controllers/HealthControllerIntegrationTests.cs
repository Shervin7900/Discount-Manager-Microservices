using FluentAssertions;
using System.Net;
using Moq;
using Moq.Protected;


namespace BaseApi.IntegrationTests.Controllers;

public class HealthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public HealthControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_HealthEndpoint_ReturnsOkAndHealthyStatus()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/health");

        // Assert
        response.EnsureSuccessStatusCode(); 
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
    }
}
