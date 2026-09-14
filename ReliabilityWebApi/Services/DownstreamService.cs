using System.Net;
using System.Text.Json;
using Polly;
using Polly.CircuitBreaker;
using Polly.Registry;
using Polly.Timeout;
using ReliabilityWebApi.Models;

namespace ReliabilityWebApi.Services;

public interface IDownstreamService
{
    Task<(int statusCode, string content, bool isFallback)> CallDownstreamWithResilienceAsync(string endpoint, CancellationToken ct);
    Task<List<CatalogItem>> GetCatalogWithGracefulDegradationAsync(CancellationToken ct);
}

public class DownstreamService : IDownstreamService
{
    private readonly HttpClient _httpClient;
    private readonly ResiliencePipeline<HttpResponseMessage> _pipeline;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<DownstreamService> _logger;

    private static readonly List<CatalogItem> StaticFallbackCatalog = new()
    {
        new CatalogItem("FALLBACK-1", "Essential Emergency Item (Cached)", 19.99m, IsFromCache: true),
        new CatalogItem("FALLBACK-2", "Offline Basic Service Pack", 49.99m, IsFromCache: true)
    };

    public DownstreamService(
        HttpClient httpClient,
        ResiliencePipelineProvider<string> pipelineProvider,
        IHttpContextAccessor httpContextAccessor,
        ILogger<DownstreamService> logger)
    {
        _httpClient = httpClient;
        _pipeline = pipelineProvider.GetPipeline<HttpResponseMessage>(ResiliencePipelines.ExternalResiliencePipeline);
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    private string ResolveFullUrl(string endpoint)
    {
        if (endpoint.StartsWith("http://", StringComparison.OrdinalIgnoreCase) || 
            endpoint.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        {
            return endpoint;
        }

        var req = _httpContextAccessor.HttpContext?.Request;
        var baseUrl = req != null ? $"{req.Scheme}://{req.Host}" : "http://localhost:5000";
        return $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
    }

    public async Task<(int statusCode, string content, bool isFallback)> CallDownstreamWithResilienceAsync(string endpoint, CancellationToken ct)
    {
        var targetUrl = ResolveFullUrl(endpoint);
        try
        {
            var response = await _pipeline.ExecuteAsync(async state =>
            {
                return await _httpClient.GetAsync(targetUrl, state);
            }, ct);

            var body = await response.Content.ReadAsStringAsync(ct);
            return ((int)response.StatusCode, body, false);
        }
        catch (BrokenCircuitException ex)
        {
            _logger.LogError("Circuit Breaker is OPEN. Fast-failing downstream call. Error: {Msg}", ex.Message);
            return (StatusCodes.Status503ServiceUnavailable, 
                JsonSerializer.Serialize(new { error = "Circuit Breaker is OPEN. Request failed fast to protect downstream service.", reason = ex.Message }), 
                true);
        }
        catch (TimeoutRejectedException ex)
        {
            _logger.LogError("Downstream call exceeded timeout. Error: {Msg}", ex.Message);
            return (StatusCodes.Status504GatewayTimeout, 
                JsonSerializer.Serialize(new { error = "Request timed out per resilience deadline policy.", reason = ex.Message }), 
                true);
        }
        catch (Exception ex)
        {
            _logger.LogError("Downstream call failed with exception: {Msg}", ex.Message);
            return (StatusCodes.Status500InternalServerError, 
                JsonSerializer.Serialize(new { error = "Downstream execution failed.", details = ex.Message }), 
                true);
        }
    }

    public async Task<List<CatalogItem>> GetCatalogWithGracefulDegradationAsync(CancellationToken ct)
    {
        var targetUrl = ResolveFullUrl("/api/simulator/catalog");
        try
        {
            var response = await _pipeline.ExecuteAsync(async state =>
            {
                return await _httpClient.GetAsync(targetUrl, state);
            }, ct);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync(ct);
                return JsonSerializer.Deserialize<List<CatalogItem>>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) 
                       ?? StaticFallbackCatalog;
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Downstream live catalog unavailable ({Msg}). Returning degraded fallback catalog.", ex.Message);
        }

        // Graceful degradation fallback
        return StaticFallbackCatalog;
    }
}
