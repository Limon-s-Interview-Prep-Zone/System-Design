using System.Security.Cryptography;
using System.Text;
using ReliabilityWebApi.Services;

namespace ReliabilityWebApi.Middleware;

public class IdempotencyMiddleware
{
    private const string IdempotencyHeader = "Idempotency-Key";
    private readonly RequestDelegate _next;
    private readonly ILogger<IdempotencyMiddleware> _logger;

    public IdempotencyMiddleware(RequestDelegate next, ILogger<IdempotencyMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IIdempotencyStore store)
    {
        // Only apply idempotency to state-mutating requests (POST, PATCH) with the header
        if (!HttpMethods.IsPost(context.Request.Method) && !HttpMethods.IsPatch(context.Request.Method))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue(IdempotencyHeader, out var keyValues) || 
            string.IsNullOrWhiteSpace(keyValues.FirstOrDefault()))
        {
            await _next(context);
            return;
        }

        var key = keyValues.First()!;
        context.Request.EnableBuffering();

        string bodyText;
        using (var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true))
        {
            bodyText = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;
        }

        var requestHash = ComputeHash(context.Request.Path + bodyText);

        var (acquired, existing) = await store.TryAcquireKeyAsync(key, requestHash, TimeSpan.FromMinutes(10));

        if (!acquired && existing != null)
        {
            if (existing.Status == IdempotencyRecordStatus.InProgress)
            {
                _logger.LogWarning("Concurrent request detected for Idempotency-Key: {Key}", key);
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"A request with this Idempotency-Key is currently in progress. Please retry shortly.\"}");
                return;
            }

            if (existing.RequestHash != requestHash)
            {
                _logger.LogWarning("Idempotency key reuse with different payload. Key: {Key}", key);
                context.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\": \"Idempotency-Key cannot be reused with a different request payload.\"}");
                return;
            }

            _logger.LogInformation("Returning cached idempotent response for Key: {Key}", key);
            context.Response.StatusCode = existing.StatusCode;
            context.Response.ContentType = existing.ContentType;
            context.Response.Headers.Append("X-Cache-Lookup", "HIT-IDEMPOTENCY");
            await context.Response.WriteAsync(existing.ResponseBody);
            return;
        }

        // Key acquired; capture outgoing response
        var originalBodyStream = context.Response.Body;
        using var memoryStream = new MemoryStream();
        context.Response.Body = memoryStream;

        try
        {
            await _next(context);

            memoryStream.Position = 0;
            var responseBody = await new StreamReader(memoryStream).ReadToEndAsync();
            memoryStream.Position = 0;

            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                await store.SaveResultAsync(
                    key,
                    context.Response.StatusCode,
                    responseBody,
                    context.Response.ContentType ?? "application/json"
                );
            }
            else
            {
                // Non-success results release key so client can retry safely
                await store.ReleaseKeyAsync(key);
            }

            await memoryStream.CopyToAsync(originalBodyStream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception during idempotent request processing for Key: {Key}. Releasing key.", key);
            await store.ReleaseKeyAsync(key);
            throw;
        }
        finally
        {
            context.Response.Body = originalBodyStream;
        }
    }

    private static string ComputeHash(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes);
    }
}
