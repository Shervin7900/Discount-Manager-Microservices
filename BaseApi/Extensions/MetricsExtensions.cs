using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace BaseApi.Extensions;

public static class MetricsExtensions
{
    public static void AddVectorMetrics(this IServiceCollection services)
    {
        services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService("DiscountManager.Api"))
            .WithMetrics(metrics =>
            {
                metrics.AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddRuntimeInstrumentation()
                    .AddPrometheusExporter();
            });
    }

    public static void UseVectorMetrics(this IApplicationBuilder app)
    {
        app.UseOpenTelemetryPrometheusScrapingEndpoint();
    }
}
