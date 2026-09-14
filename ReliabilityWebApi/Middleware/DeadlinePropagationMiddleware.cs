namespace ReliabilityWebApi.Middleware;

public class DeadlinePropagationMiddleware
{
    private const string DeadlineHeader = "X-Request-Deadline-Ms";
    private readonly RequestDelegate _next;
    private readonly ILogger<DeadlinePropagationMiddleware> _logger;

    public DeadlinePropagationMiddleware(RequestDelegate next, ILogger<DeadlinePropagationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(DeadlineHeader, out var headerVal) &&
            int.TryParse(headerVal.FirstOrDefault(), out var deadlineMs))
        {
            if (deadlineMs <= 0)
            {
                _logger.LogWarning("Request dropped: Incoming deadline {DeadlineMs}ms already expired", deadlineMs);
                context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"Request deadline already expired before processing.\"}");
                return;
            }

            using var cts = CancellationTokenSource.CreateLinkedTokenSource(context.RequestAborted);
            cts.CancelAfter(TimeSpan.FromMilliseconds(deadlineMs));

            var originalToken = context.RequestAborted;
            context.RequestAborted = cts.Token;

            try
            {
                await _next(context);
            }
            catch (OperationCanceledException) when (cts.IsCancellationRequested && !originalToken.IsCancellationRequested)
            {
                _logger.LogWarning("Request aborted due to deadline exceeded ({DeadlineMs}ms)", deadlineMs);
                if (!context.Response.HasStarted)
                {
                    context.Response.StatusCode = StatusCodes.Status504GatewayTimeout;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync("{\"error\": \"End-to-end request deadline exceeded during processing.\"}");
                }
            }
            finally
            {
                context.RequestAborted = originalToken;
            }

            return;
        }

        await _next(context);
    }
}
