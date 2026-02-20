using FluentAssertions;
using System.Net;

namespace BaseApi.IntegrationTests.Controllers;

public class VectorDashboardControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public VectorDashboardControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_VectorDashboardEndpoint_ReturnsUnauthorized_WhenNoToken()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/vectordashboard");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
