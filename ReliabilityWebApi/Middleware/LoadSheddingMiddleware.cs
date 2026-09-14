namespace ReliabilityWebApi.Middleware;

public class LoadSheddingMiddleware
{
    private static int _activeRequests = 0;
    private const int MaxConcurrentRequests = 50; // Threshold for load shedding demo
    private readonly RequestDelegate _next;
    private readonly ILogger<LoadSheddingMiddleware> _logger;

    public LoadSheddingMiddleware(RequestDelegate next, ILogger<LoadSheddingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public static int ActiveRequests => _activeRequests;

    public async Task InvokeAsync(HttpContext context)
    {
        // Don't shed health checks or chaos configuration
        if (context.Request.Path.StartsWithSegments("/health") || 
            context.Request.Path.StartsWithSegments("/api/chaos"))
        {
            await _next(context);
            return;
        }

        var current = Interlocked.Increment(ref _activeRequests);
        try
        {
            if (current > MaxConcurrentRequests)
            {
                _logger.LogWarning("Load shedding triggered! Active requests: {Count} > Max: {Max}", current, MaxConcurrentRequests);
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                context.Response.Headers.Append("Retry-After", "3");
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"System overload. Request dropped by Load Shedder to prevent cascading failure.\"}");
                return;
            }

            await _next(context);
        }
        finally
        {
            Interlocked.Decrement(ref _activeRequests);
        }
    }
}
