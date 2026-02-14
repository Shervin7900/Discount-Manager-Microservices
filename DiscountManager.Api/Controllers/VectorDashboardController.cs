using Microsoft.AspNetCore.Mvc;

namespace DiscountManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VectorDashboardController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;

    public VectorDashboardController(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var client = _httpClientFactory.CreateClient();
        var metrics = await client.GetStringAsync($"{Request.Scheme}://{Request.Host}/metrics");

        var html = $@"
<!DOCTYPE html>
<html>
<head>
    <title>Vector Metrics Dashboard</title>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #1e1e2e; color: #cdd6f4; margin: 20px; }}
        h1 {{ color: #fab387; border-bottom: 2px solid #fab387; padding-bottom: 10px; }}
        pre {{ background-color: #313244; padding: 20px; border-radius: 8px; overflow-x: auto; font-size: 14px; border: 1px solid #45475a; }}
        .metric-item {{ margin-bottom: 20px; }}
        .footer {{ margin-top: 30px; font-size: 12px; color: #7f849c; }}
    </style>
</head>
<body>
    <h1>🚀 Vector Metrics Dashboard</h1>
    <p>Live metrics from the OpenTelemetry Prometheus endpoint.</p>
    <div class='metric-item'>
        <pre>{metrics}</pre>
    </div>
    <div class='footer'>Scraped at: {DateTime.Now}</div>
    <script>
        setTimeout(() => location.reload(), 5000);
    </script>
</body>
</html>";

        return Content(html, "text/html");
    }
}
