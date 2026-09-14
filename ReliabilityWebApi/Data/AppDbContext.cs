using Microsoft.EntityFrameworkCore;
using ReliabilityWebApi.Models;

namespace ReliabilityWebApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Account> Accounts => Set<Account>();
    public DbSet<Payment> Payments => Set<Payment>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<IdempotencyEntity> IdempotencyRecords => Set<IdempotencyEntity>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Account mapping & constraints
        modelBuilder.Entity<Account>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.AccountId).IsUnique();
        });

        // Payment mapping & constraints
        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.TransactionId).IsUnique();
            entity.Property(e => e.Amount).HasPrecision(18, 2);
        });

        // Product mapping & constraints
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasPrecision(18, 2);
        });

        // IdempotencyEntity mapping & constraints
        modelBuilder.Entity<IdempotencyEntity>(entity =>
        {
            entity.HasKey(e => e.Key);
        });

        // -------------------------------------------------------------
        // SEED DATA: Exactly between 2 and 4 records per entity
        // -------------------------------------------------------------

        // 1. Seed Accounts (3 entities)
        modelBuilder.Entity<Account>().HasData(
            new Account
            {
                Id = 1,
                AccountId = "ACC-101",
                OwnerName = "Alice Enterprise Corp",
                Balance = 150000.00m,
                Currency = "USD",
                CreatedAt = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            },
            new Account
            {
                Id = 2,
                AccountId = "ACC-102",
                OwnerName = "Bob Logistics LLC",
                Balance = 84500.50m,
                Currency = "USD",
                CreatedAt = new DateTime(2026, 1, 15, 0, 0, 0, DateTimeKind.Utc)
            },
            new Account
            {
                Id = 3,
                AccountId = "ACC-103",
                OwnerName = "Charlie Cloud Inc",
                Balance = 32000.00m,
                Currency = "EUR",
                CreatedAt = new DateTime(2026, 2, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );

        // 2. Seed Payments (3 entities)
        modelBuilder.Entity<Payment>().HasData(
            new Payment
            {
                Id = 1,
                TransactionId = "TX-SEED-1001",
                AccountId = "ACC-101",
                Amount = 12500.00m,
                Currency = "USD",
                Status = "SETTLED",
                ProcessedAt = new DateTime(2026, 2, 10, 14, 30, 0, DateTimeKind.Utc),
                Description = "Dedicated Server Infrastructure Q1"
            },
            new Payment
            {
                Id = 2,
                TransactionId = "TX-SEED-1002",
                AccountId = "ACC-102",
                Amount = 450.75m,
                Currency = "USD",
                Status = "SETTLED",
                ProcessedAt = new DateTime(2026, 2, 12, 09, 15, 0, DateTimeKind.Utc),
                Description = "High-Availability Load Balancer Subscription"
            },
            new Payment
            {
                Id = 3,
                TransactionId = "TX-SEED-1003",
                AccountId = "ACC-103",
                Amount = 2990.00m,
                Currency = "EUR",
                Status = "SETTLED",
                ProcessedAt = new DateTime(2026, 2, 14, 11, 45, 0, DateTimeKind.Utc),
                Description = "Database Multi-AZ Replication Tier"
            }
        );

        // 3. Seed Products / Catalog Items (3 entities)
        modelBuilder.Entity<Product>().HasData(
            new Product
            {
                Id = "PROD-101",
                Name = "Resilient Kubernetes Cluster (High Availability)",
                Price = 899.00m,
                StockQuantity = 50,
                IsActive = true
            },
            new Product
            {
                Id = "PROD-102",
                Name = "Global Anycast CDN & DDoS Shield",
                Price = 299.00m,
                StockQuantity = 100,
                IsActive = true
            },
            new Product
            {
                Id = "PROD-103",
                Name = "Geo-Replicated Distributed Cache Tier",
                Price = 450.00m,
                StockQuantity = 30,
                IsActive = true
            }
        );

        // 4. Seed IdempotencyRecords (2 entities)
        modelBuilder.Entity<IdempotencyEntity>().HasData(
            new IdempotencyEntity
            {
                Key = "seed-idempotency-key-001",
                RequestHash = "HASH-SEED-001",
                Status = "Completed",
                StatusCode = 200,
                ResponseBody = "{\"transactionId\":\"TX-SEED-1001\",\"status\":\"SETTLED\",\"amount\":12500.00,\"currency\":\"USD\",\"message\":\"Seeded idempotent transaction.\"}",
                ContentType = "application/json",
                CreatedAt = new DateTime(2026, 2, 10, 14, 30, 0, DateTimeKind.Utc),
                ExpiresAt = new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc)
            },
            new IdempotencyEntity
            {
                Key = "seed-idempotency-key-002",
                RequestHash = "HASH-SEED-002",
                Status = "Completed",
                StatusCode = 200,
                ResponseBody = "{\"transactionId\":\"TX-SEED-1002\",\"status\":\"SETTLED\",\"amount\":450.75,\"currency\":\"USD\",\"message\":\"Seeded idempotent transaction.\"}",
                ContentType = "application/json",
                CreatedAt = new DateTime(2026, 2, 12, 09, 15, 0, DateTimeKind.Utc),
                ExpiresAt = new DateTime(2026, 12, 31, 23, 59, 59, DateTimeKind.Utc)
            }
        );
    }
}
