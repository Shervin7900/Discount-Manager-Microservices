using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Text.Json;

namespace BaseApi.IntegrationTests;

public class IdentityServerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;

    public IdentityServerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task OpenIdConfiguration_ReturnsIdentityServerMetadata()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/.well-known/openid-configuration");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new Exception($"OIDC config failed with {response.StatusCode}. Content: {content}");
        }

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = JsonDocument.Parse(content);
        
        json.RootElement.GetProperty("issuer").GetString().Should().NotBeNullOrEmpty();
        json.RootElement.GetProperty("jwks_uri").GetString().Should().Contain("jwks");
    }

    [Fact]
    public async Task DiscoveryEndpoint_IsAccessible()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/.well-known/openid-configuration");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("token_endpoint");
    }

    [Fact]
    public async Task RequestToken_ReturnsValidJwt()
    {
        // Arrange
        var client = _factory.CreateClient();
        var disco = await client.GetAsync("/.well-known/openid-configuration");
        disco.EnsureSuccessStatusCode();

        // Act
        var requestContent = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("client_id", "client"),
            new KeyValuePair<string, string>("client_secret", "secret"),
            new KeyValuePair<string, string>("grant_type", "client_credentials"),
            new KeyValuePair<string, string>("scope", "discount.read")
        });

        var response = await client.PostAsync("/connect/token", requestContent);

        // Assert
        var responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Token request failed with {response.StatusCode}. Content: {responseContent}");
        }

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = JsonDocument.Parse(responseContent);
        json.RootElement.GetProperty("access_token").GetString().Should().NotBeNullOrEmpty();
        json.RootElement.GetProperty("token_type").GetString().Should().Be("Bearer");
    }
}
