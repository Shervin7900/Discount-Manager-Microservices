using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace BaseApi.Infrastructure.Middlewares;

public class LoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LoggingMiddleware> _logger;

    public byte[]? RequestBody { get; private set; }

    public LoggingMiddleware(RequestDelegate next, ILogger<LoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log Request
        context.Request.EnableBuffering();
        var requestBodyStream = new MemoryStream();
        await context.Request.Body.CopyToAsync(requestBodyStream);
        requestBodyStream.Seek(0, SeekOrigin.Begin);
        var requestBodyText = Encoding.UTF8.GetString(requestBodyStream.ToArray());
        context.Request.Body.Seek(0, SeekOrigin.Begin);

        _logger.LogInformation("HTTP Request Information: {Method} {Path} {Body}", 
            context.Request.Method, context.Request.Path, requestBodyText);

        // Continue pipeline
        await _next(context);

        // Log Response
        _logger.LogInformation("HTTP Response Information: {StatusCode}", context.Response.StatusCode);
    }
}
