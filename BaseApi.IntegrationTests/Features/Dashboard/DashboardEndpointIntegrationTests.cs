using FluentAssertions;
using System.Net;

namespace BaseApi.IntegrationTests.Features.Dashboard;

public class DashboardEndpointIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public DashboardEndpointIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Get_DashboardEndpoint_ReturnsOk_WhenAllowAnonymous()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/dashboard");

        // Assert
        // Currently AllowAnonymous() is set in the endpoint for simplicity
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
