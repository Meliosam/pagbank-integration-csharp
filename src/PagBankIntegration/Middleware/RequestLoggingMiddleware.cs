using System.Diagnostics;

namespace PagBankIntegration.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate                    _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw        = Stopwatch.StartNew();
        var requestId = Guid.NewGuid().ToString("N")[..8].ToUpper();
        context.Response.Headers["X-Request-Id"] = requestId;
        _logger.LogInformation("[{RequestId}] >> {Method} {Path}",
            requestId, context.Request.Method, context.Request.Path);
        try   { await _next(context); }
        finally
        {
            sw.Stop();
            _logger.LogInformation("[{RequestId}] << {Status} ^({Elapsed}ms^)",
                requestId, context.Response.StatusCode, sw.ElapsedMilliseconds);
        }
    }
}
