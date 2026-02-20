using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net;
using Testcontainers.MongoDb;
using Testcontainers.MsSql;
using Testcontainers.Redis;

namespace BaseApi.E2ETests;

public class SystemFlowTests : IAsyncLifetime
{
    private readonly MsSqlContainer _msSqlContainer;
    private readonly RedisContainer _redisContainer;
    private readonly MongoDbContainer _mongoDbContainer;
    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public SystemFlowTests()
    {
        _msSqlContainer = new MsSqlBuilder().Build();
        _redisContainer = new RedisBuilder().Build();
        _mongoDbContainer = new MongoDbBuilder().Build();
    }

    public async Task InitializeAsync()
    {
        await Task.WhenAll(
            _msSqlContainer.StartAsync(),
            _redisContainer.StartAsync(),
            _mongoDbContainer.StartAsync()
        );

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["ConnectionStrings:DefaultConnection"] = _msSqlContainer.GetConnectionString(),
                    ["ConnectionStrings:Redis"] = _redisContainer.GetConnectionString(),
                    ["ConnectionStrings:Mongo"] = _mongoDbContainer.GetConnectionString()
                });
            });
        });

        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        await Task.WhenAll(
            _msSqlContainer.DisposeAsync().AsTask(),
            _redisContainer.DisposeAsync().AsTask(),
            _mongoDbContainer.DisposeAsync().AsTask()
        );
        _factory.Dispose();
        _client.Dispose();
    }

    [Fact]
    public async Task GetHealth_ReturnsOk_WithRealDatabases()
    {
        // Act
        var response = await _client.GetAsync("/api/health");
        var content = await response.Content.ReadAsStringAsync();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK, "Response content: " + content);
        content.Should().Contain("Healthy");
    }
}
