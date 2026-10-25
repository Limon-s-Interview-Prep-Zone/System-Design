using Microsoft.EntityFrameworkCore;
using ReliabilityWebApi.Data;
using ReliabilityWebApi.Models;

namespace ReliabilityWebApi.Services;

public class EfCoreIdempotencyStore : IIdempotencyStore
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<EfCoreIdempotencyStore> _logger;

    public EfCoreIdempotencyStore(IServiceScopeFactory scopeFactory, ILogger<EfCoreIdempotencyStore> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    public async Task<(bool acquired, IdempotencyRecord? existingRecord)> TryAcquireKeyAsync(
        string key, 
        string requestHash, 
        TimeSpan ttl)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var now = DateTime.UtcNow;
        var existing = await db.IdempotencyRecords.FirstOrDefaultAsync(x => x.Key == key);

        if (existing != null)
        {
            // If expired, remove and re-acquire
            if (existing.ExpiresAt <= now)
            {
                db.IdempotencyRecords.Remove(existing);
                await db.SaveChangesAsync();
            }
            else
            {
                var record = new IdempotencyRecord
                {
                    Key = existing.Key,
                    RequestHash = existing.RequestHash,
                    Status = Enum.Parse<IdempotencyRecordStatus>(existing.Status),
                    StatusCode = existing.StatusCode,
                    ResponseBody = existing.ResponseBody,
                    ContentType = existing.ContentType,
                    CreatedAt = existing.CreatedAt,
                    ExpiresAt = existing.ExpiresAt
                };
                return (false, record);
            }
        }

        var newEntity = new IdempotencyEntity
        {
            Key = key,
            RequestHash = requestHash,
            Status = IdempotencyRecordStatus.InProgress.ToString(),
            StatusCode = 0,
            ResponseBody = string.Empty,
            ContentType = "application/json",
            CreatedAt = now,
            ExpiresAt = now.Add(ttl)
        };

        try
        {
            db.IdempotencyRecords.Add(newEntity);
            await db.SaveChangesAsync();
            return (true, null);
        }
        catch (DbUpdateException ex)
        {
            _logger.LogWarning("Concurrent insert collision for Idempotency-Key {Key}: {Msg}", key, ex.Message);
            var current = await db.IdempotencyRecords.AsNoTracking().FirstOrDefaultAsync(x => x.Key == key);
            if (current != null)
            {
                var record = new IdempotencyRecord
                {
                    Key = current.Key,
                    RequestHash = current.RequestHash,
                    Status = Enum.Parse<IdempotencyRecordStatus>(current.Status),
                    StatusCode = current.StatusCode,
                    ResponseBody = current.ResponseBody,
                    ContentType = current.ContentType,
                    CreatedAt = current.CreatedAt,
                    ExpiresAt = current.ExpiresAt
                };
                return (false, record);
            }
            throw;
        }
    }

    public async Task SaveResultAsync(
        string key, 
        int statusCode, 
        string responseBody, 
        string contentType)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entity = await db.IdempotencyRecords.FirstOrDefaultAsync(x => x.Key == key);
        if (entity != null)
        {
            entity.Status = IdempotencyRecordStatus.Completed.ToString();
            entity.StatusCode = statusCode;
            entity.ResponseBody = responseBody;
            entity.ContentType = contentType;
            await db.SaveChangesAsync();
        }
    }

    public async Task ReleaseKeyAsync(string key)
    {
        using var scope = _scopeFactory.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        var entity = await db.IdempotencyRecords.FirstOrDefaultAsync(x => x.Key == key);
        if (entity != null)
        {
            db.IdempotencyRecords.Remove(entity);
            await db.SaveChangesAsync();
        }
    }
}
