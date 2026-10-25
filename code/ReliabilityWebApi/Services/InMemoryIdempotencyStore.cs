using System.Collections.Concurrent;

namespace ReliabilityWebApi.Services;

public enum IdempotencyRecordStatus
{
    InProgress,
    Completed,
    Failed
}

public class IdempotencyRecord
{
    public string Key { get; set; } = string.Empty;
    public string RequestHash { get; set; } = string.Empty;
    public IdempotencyRecordStatus Status { get; set; }
    public int StatusCode { get; set; }
    public string ResponseBody { get; set; } = string.Empty;
    public string ContentType { get; set; } = "application/json";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
}

public interface IIdempotencyStore
{
    Task<(bool acquired, IdempotencyRecord? existingRecord)> TryAcquireKeyAsync(
        string key, 
        string requestHash, 
        TimeSpan ttl);

    Task SaveResultAsync(
        string key, 
        int statusCode, 
        string responseBody, 
        string contentType);

    Task ReleaseKeyAsync(string key);
}

public class InMemoryIdempotencyStore : IIdempotencyStore
{
    private readonly ConcurrentDictionary<string, IdempotencyRecord> _store = new();

    public Task<(bool acquired, IdempotencyRecord? existingRecord)> TryAcquireKeyAsync(
        string key, 
        string requestHash, 
        TimeSpan ttl)
    {
        var now = DateTime.UtcNow;

        // Clean up expired entry if any
        if (_store.TryGetValue(key, out var current) && current.ExpiresAt <= now)
        {
            _store.TryRemove(key, out _);
        }

        var newRecord = new IdempotencyRecord
        {
            Key = key,
            RequestHash = requestHash,
            Status = IdempotencyRecordStatus.InProgress,
            CreatedAt = now,
            ExpiresAt = now.Add(ttl)
        };

        var actual = _store.GetOrAdd(key, newRecord);

        if (ReferenceEquals(actual, newRecord))
        {
            // Successfully acquired lock
            return Task.FromResult((true, (IdempotencyRecord?)null));
        }

        // Key already exists
        return Task.FromResult((false, (IdempotencyRecord?)actual));
    }

    public Task SaveResultAsync(
        string key, 
        int statusCode, 
        string responseBody, 
        string contentType)
    {
        if (_store.TryGetValue(key, out var record))
        {
            record.Status = IdempotencyRecordStatus.Completed;
            record.StatusCode = statusCode;
            record.ResponseBody = responseBody;
            record.ContentType = contentType;
        }

        return Task.CompletedTask;
    }

    public Task ReleaseKeyAsync(string key)
    {
        _store.TryRemove(key, out _);
        return Task.CompletedTask;
    }
}
