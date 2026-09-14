using System.Net;
using System.Threading.RateLimiting;
using Polly;
using Polly.CircuitBreaker;
using Polly.Retry;
using Polly.Timeout;
using ReliabilityWebApi.Models;

namespace ReliabilityWebApi.Services;

public static class ResiliencePipelines
{
    public const string ExternalResiliencePipeline = "external-resilience-pipeline";
    public const string CatalogFallbackPipeline = "catalog-fallback-pipeline";
    public const string BulkheadVaultPipeline = "bulkhead-vault-pipeline";

    public static IServiceCollection AddAppResiliencePipelines(
        this IServiceCollection services, 
        ChaosSimulationState chaosState,
        ILogger logger)
    {
        services.AddResiliencePipeline<string, HttpResponseMessage>(ExternalResiliencePipeline, builder =>
        {
            // 1. Total request timeout
            builder.AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromSeconds(6),
                OnTimeout = args =>
                {
                    logger.LogWarning("[Resilience] Total request timeout of {Timeout} reached", args.Timeout);
                    return default;
                }
            });

            // 2. Retry with Exponential Backoff + Full Jitter
            builder.AddRetry(new RetryStrategyOptions<HttpResponseMessage>
            {
                MaxRetryAttempts = 3,
                BackoffType = DelayBackoffType.Exponential,
                UseJitter = true,
                Delay = TimeSpan.FromMilliseconds(200),
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .Handle<TimeoutRejectedException>()
                    .HandleResult(res => (int)res.StatusCode >= 500 || res.StatusCode == HttpStatusCode.RequestTimeout),
                OnRetry = args =>
                {
                    var delay = args.RetryDelay.TotalMilliseconds;
                    var reason = args.Outcome.Exception?.Message ?? $"HTTP {(int)(args.Outcome.Result?.StatusCode ?? 0)}";
                    logger.LogWarning("[Resilience] Retry attempt {Attempt} after {Delay}ms backoff (with Jitter). Reason: {Reason}",
                        args.AttemptNumber + 1, delay, reason);
                    chaosState.RecordEvent("RetryTriggered", $"Attempt {args.AttemptNumber + 1}, Backoff: {delay:F1}ms, Cause: {reason}");
                    return default;
                }
            });

            // 3. Circuit Breaker Strategy
            builder.AddCircuitBreaker(new CircuitBreakerStrategyOptions<HttpResponseMessage>
            {
                FailureRatio = 0.5, // 50% failure rate trips breaker
                SamplingDuration = TimeSpan.FromSeconds(10),
                MinimumThroughput = 4,
                BreakDuration = TimeSpan.FromSeconds(10),
                ShouldHandle = new PredicateBuilder<HttpResponseMessage>()
                    .Handle<HttpRequestException>()
                    .Handle<TimeoutRejectedException>()
                    .HandleResult(res => (int)res.StatusCode >= 500),
                OnOpened = args =>
                {
                    logger.LogError("[CircuitBreaker] Breaker TRIPPED to OPEN! Break duration: {Duration}s", args.BreakDuration.TotalSeconds);
                    chaosState.RecordEvent("CircuitBreaker_OPEN", $"Tripped for {args.BreakDuration.TotalSeconds}s due to failure ratio > 50%");
                    return default;
                },
                OnClosed = args =>
                {
                    logger.LogInformation("[CircuitBreaker] Breaker RESET to CLOSED. Traffic flowing normally.");
                    chaosState.RecordEvent("CircuitBreaker_CLOSED", "Probes succeeded. Breaker closed.");
                    return default;
                },
                OnHalfOpened = args =>
                {
                    logger.LogInformation("[CircuitBreaker] Breaker in HALF-OPEN state. Testing trial requests...");
                    chaosState.RecordEvent("CircuitBreaker_HALF_OPEN", "Cooldown elapsed. Allowing canary trial requests.");
                    return default;
                }
            });

            // 4. Per-try timeout
            builder.AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromMilliseconds(1500),
                OnTimeout = args =>
                {
                    logger.LogWarning("[Resilience] Per-try call timed out after {Timeout}ms", args.Timeout.TotalMilliseconds);
                    return default;
                }
            });
        });

        // 5. Bulkhead (Concurrency Limiter) Pipeline for critical resources (e.g. Card Vault)
        services.AddResiliencePipeline(BulkheadVaultPipeline, builder =>
        {
            builder.AddConcurrencyLimiter(new ConcurrencyLimiterOptions
            {
                PermitLimit = 2,           // Max 2 concurrent executions allowed
                QueueLimit = 1,            // Max 1 queued
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst
            });
        });

        return services;
    }
}
