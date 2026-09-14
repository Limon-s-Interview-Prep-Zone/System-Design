# Implementation Plan: .NET Core Web API for Reliability Patterns

This plan outlines the design and implementation of a complete, runnable ASP.NET Core Web API project (`ReliabilityWebApi`) demonstrating all the resilience and reliability patterns documented in [`plans/reliability-guide.md`](file:///D:/Interview/System-Design/plans/reliability-guide.md).

---

## 1. Project Specifications
- **Project Name:** `ReliabilityWebApi`
- **Location:** `D:/Interview/System-Design/ReliabilityWebApi`
- **Target Framework:** `net8.0` (LTS, supported by installed .NET SDK 10)
- **Primary Libraries:**
  - `Microsoft.Extensions.Http.Resilience` (Polly v8 resilience pipelines)
  - `Microsoft.AspNetCore.RateLimiting` (built-in .NET 8 rate limiting & concurrency limiters)
  - `AspNetCore.HealthChecks.UI.Client` (health check endpoints)
  - OpenTelemetry / System.Diagnostics.Metrics (RED signals & observability)
  - Always support openapi
  - Always structure project MVC format webapi
  - Always EF Core ORM
  - Always sqllite.db for fasster development
  - ALways use seed data for each entity at least 2 and atmost 4

---

## 2. Architecture & Components

ReliabilityWebApi/
├── Controllers/
│   ├── PaymentsController.cs          # Demonstrates Idempotency & Bulkhead isolation, queries SQLite
│   ├── ResilienceDemoController.cs    # Demonstrates Timeouts, Retries (Backoff+Jitter), Circuit Breaker & Fallbacks
│   ├── DownstreamSimulatorController.cs # Simulates downstream dependencies, queries SQLite catalog
│   ├── ChaosController.cs             # Dynamic runtime fault injection & event tracking
├── Data/
│   ├── AppDbContext.cs                # EF Core DbContext with SQLite (sqlite.db) & seeded entities
├── Middleware/
│   ├── IdempotencyMiddleware.cs       # Validates Idempotency-Key, stores responses in SQLite
│   ├── LoadSheddingMiddleware.cs      # Detects high system load and rejects low-priority requests (HTTP 503)
│   ├── DeadlinePropagationMiddleware.cs # Propagates X-Request-Deadline-Ms to request tokens
├── Services/
│   ├── IDownstreamService.cs          # Interface for resilient downstream calls & graceful degradation
│   ├── DownstreamService.cs           # Downstream client with Polly v8 Resilience Pipeline
│   ├── IIdempotencyStore.cs           # Idempotency store contract
│   ├── EfCoreIdempotencyStore.cs      # EF Core + SQLite persistent idempotency store
│   ├── ChaosSimulationState.cs        # Thread-safe fault injection state & event log
│   ├── ResiliencePipelines.cs         # Polly v8 policies (Timeout, Retry, Circuit Breaker, Bulkhead)
├── Models/
│   ├── Entities.cs                    # EF Core entities (Account, Payment, Product, IdempotencyEntity)
│   ├── PaymentModels.cs               # API DTOs and view models
├── HealthChecks/
│   ├── DownstreamHealthCheck.cs       # Readiness health check
├── Program.cs                         # Wires up Rate Limiter, Health Checks, SQLite, Polly pipelines
├── sqlite.db                          # Embedded SQLite database (WAL mode enabled)
└── ReliabilityWebApi.csproj
```

---

## 3. Pattern Implementation Matrix

| Pattern from Guide | Implementation Mechanism | Test Endpoint |
| :--- | :--- | :--- |
| **Timeouts & Deadlines** | `Polly.Timeout` + CancellationToken linked with `X-Request-Deadline` | `GET /api/external/timeout-demo` |
| **Retries + Exponential Backoff + Full Jitter** | Polly Resilience Pipeline with `RetryStrategyOptions`, `BackoffType.Exponential`, `UseJitter = true` | `GET /api/external/retry-demo` |
| **Circuit Breaker** | Polly `CircuitBreakerStrategyOptions` (State: Closed, Open, HalfOpen) with fallback | `GET /api/external/circuit-breaker-demo` |
| **Bulkhead (Resource Isolation)** | Concurrency Limiter & Polly `RateLimiterStrategyOptions` | `POST /api/payments/checkout` |
| **Rate Limiting (Token / Sliding Window)** | ASP.NET Core `AddRateLimiter` (Token Bucket / Sliding Window) | `GET /api/external/rate-limited` |
| **Load Shedding** | Middleware checking active request count/concurrency threshold | `GET /api/external/load-shedding-demo` |
| **Idempotency** | Header `Idempotency-Key` + deduplication middleware + safe atomic state | `POST /api/payments/charge` |
| **Graceful Degradation / Fallbacks** | Polly Fallback pipeline returning cached/static payload when circuit opens | `GET /api/external/degraded-catalog` |
| **Health Checks (Liveness vs Readiness)** | `/health/live` (process alive) & `/health/ready` (dependency check) | `GET /health/live`, `GET /health/ready` |
| **Chaos Engineering / Simulation** | Controlled fault injection endpoint to trigger latency, HTTP 500s | `POST /api/chaos/configure` |

---

## 4. Execution Steps
1. Create project via `dotnet new webapi -n ReliabilityWebApi -f net8.0` in `D:/Interview/System-Design/ReliabilityWebApi`.
2. Add necessary NuGet packages (`Microsoft.Extensions.Http.Resilience`).
3. Implement models, stores, middleware, and resilience pipelines.
4. Implement controllers showcasing each reliability pattern.
5. Build and verify test execution via `dotnet build` and `dotnet run`.
