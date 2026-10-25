# Reliability & Resilience Web API (.NET 8 Core)

A reference implementation demonstrating all production-grade reliability and fault-tolerance patterns from [`plans/reliability-guide.md`](file:///D:/Interview/System-Design/plans/reliability-guide.md).

---

## 🛠️ Architecture & Pattern Mapping

| Pattern | Code Implementation | Test Endpoint |
| :--- | :--- | :--- |
| **Idempotency** | [`IdempotencyMiddleware.cs`](file:///D:/Interview/System-Design/ReliabilityWebApi/Middleware/IdempotencyMiddleware.cs) & [`EfCoreIdempotencyStore.cs`](file:///D:/Interview/System-Design/ReliabilityWebApi/Services/EfCoreIdempotencyStore.cs) | `POST /api/payments/charge` with `Idempotency-Key` |
| **Database Persistence & Seed Data** | [`AppDbContext.cs`](file:///D:/Interview/System-Design/ReliabilityWebApi/Data/AppDbContext.cs) with SQLite (`sqlite.db`) | `GET /api/payments/accounts`, `GET /api/simulator/catalog` |
| **Retries + Backoff + Jitter** | [`ResiliencePipelines.cs`](file:///D:/Interview/System-Design/ReliabilityWebApi/Services/ResiliencePipelines.cs) (Polly v8 Exponential + Full Jitter) | `GET /api/resilience/retry-with-backoff` |
| **Circuit Breaker** | [`ResiliencePipelines.cs`](file:///D:/Interview/System-Design/ReliabilityWebApi/Services/ResiliencePipelines.cs) (Closed $\rightarrow$ Open $\rightarrow$ HalfOpen transitions) | `GET /api/resilience/circuit-breaker-demo` |
| **Timeout & Deadlines** | [`DeadlinePropagationMiddleware.cs`](file:///D:/Interview/System-Design/ReliabilityWebApi/Middleware/DeadlinePropagationMiddleware.cs) & Polly Timeout Strategy | `GET /api/resilience/timeout-demo` |
| **Bulkhead Isolation** | Polly Concurrency Limiter (Card Vault thread isolation) | `POST /api/payments/vault-access` |
| **Load Shedding** | [`LoadSheddingMiddleware.cs`](file:///D:/Interview/System-Design/ReliabilityWebApi/Middleware/LoadSheddingMiddleware.cs) (sheds traffic when saturated) | High concurrency on `/api/*` endpoints |
| **Rate Limiting** | ASP.NET Core `AddRateLimiter` (Token Bucket Policy) | `GET /api/resilience/rate-limited` |
| **Graceful Degradation** | [`DownstreamService.cs`](file:///D:/Interview/System-Design/ReliabilityWebApi/Services/DownstreamService.cs) (serves cached/fallback catalog) | `GET /api/resilience/graceful-degradation` |
| **Health Probes (Live vs Ready)** | ASP.NET Core Health Checks (`/health/live` vs `/health/ready`) | `GET /health/live`, `GET /health/ready` |
| **Chaos & Fault Injection** | [`ChaosSimulationState.cs`](file:///D:/Interview/System-Design/ReliabilityWebApi/Services/ChaosSimulationState.cs) & [`ChaosController.cs`](file:///D:/Interview/System-Design/ReliabilityWebApi/Controllers/ChaosController.cs) | `POST /api/chaos/configure` |

---

## 📦 Codebase Implementation Details

### 1. Data Layer (EF Core ORM & SQLite)
* **ORM:** Microsoft Entity Framework Core 8 (`Microsoft.EntityFrameworkCore.Sqlite`).
* **Database File:** Embedded [`sqlite.db`](file:///D:/Interview/System-Design/ReliabilityWebApi/sqlite.db) with Write-Ahead Logging (WAL) enabled automatically for crash durability.
* **Auto-Initialization:** Configured in `Program.cs` via `db.Database.EnsureCreated()`.
* **Seed Data (Strict constraint: 2 to 4 records per entity):**
  - **`Account` (3 entities):**
    - `ACC-101`: Alice Enterprise Corp ($150,000.00 USD)
    - `ACC-102`: Bob Logistics LLC ($84,500.50 USD)
    - `ACC-103`: Charlie Cloud Inc (€32,000.00 EUR)
  - **`Payment` (3 entities):**
    - `TX-SEED-1001`: Dedicated Server Infrastructure ($12,500.00)
    - `TX-SEED-1002`: High-Availability Load Balancer ($450.75)
    - `TX-SEED-1003`: Database Multi-AZ Replication Tier (€2,990.00)
  - **`Product` (3 entities):**
    - `PROD-101`: Resilient Kubernetes Cluster ($899.00)
    - `PROD-102`: Global Anycast CDN & DDoS Shield ($299.00)
    - `PROD-103`: Geo-Replicated Distributed Cache Tier ($450.00)
  - **`IdempotencyEntity` (2 entities):**
    - `seed-idempotency-key-001`: Pre-seeded completed idempotency record.
    - `seed-idempotency-key-002`: Pre-seeded completed idempotency record.

### 2. Middleware Pipeline
* **`DeadlinePropagationMiddleware`**: Checks `X-Request-Deadline-Ms`. If expired, rejects immediately (`504 Gateway Timeout`). Otherwise links a `CancellationTokenSource` to abort downstream operations when the deadline elapses.
* **`LoadSheddingMiddleware`**: Tracks active in-flight executions. When active requests breach safe capacity, drops low-priority traffic with `503 Service Unavailable` and `Retry-After: 3` header.
* **`IdempotencyMiddleware`**: Intercepts `Idempotency-Key` headers on state-mutating requests (`POST`). Atomically locks the key in SQLite (`InProgress`), prevents concurrent duplicates (`409 Conflict`), detects altered request payloads (`422 Unprocessable Entity`), and returns cached responses on subsequent identical calls (`X-Cache-Lookup: HIT-IDEMPOTENCY`).
* **`RateLimiter`**: Built-in ASP.NET Core token bucket rate limiter (5 tokens per 10-second window, rejecting excess traffic with `429 Too Many Requests`).

### 3. Resilience Pipelines (Polly v8)
* **Retries + Backoff + Full Jitter**: 3 retry attempts with exponential backoff and randomized jitter to prevent the "Thundering Herd" problem.
* **Circuit Breaker**: Tracks consecutive failures and failure ratios (>50% over 10s sampling window). Trips to `Open` (break duration 10s), then transitions to `Half-Open` for trial probe requests before closing.
* **Bulkhead Isolation**: Isolates sensitive resources (e.g. Card Vault) using a dedicated concurrency limiter (max 2 concurrent threads) to prevent resource starvation in high-traffic scenarios.
* **Graceful Degradation**: When live dependencies fail, the system automatically falls back to an offline/stale catalog rather than returning an error to the user.

### 4. Health Checks
* **`/health/live` (Liveness Probe)**: Checks process responsiveness. External dependencies (like databases) are intentionally omitted to avoid cascading restart storms across container clusters.
* **`/health/ready` (Readiness Probe)**: Verifies downstream dependencies and database readiness. Pulls the instance from load-balancer rotation if degraded or experiencing an outage.

### 5. File Structure
```
ReliabilityWebApi/
├── Controllers/
│   ├── PaymentsController.cs          # Idempotency, payments ledger, and bulkhead access
│   ├── ResilienceDemoController.cs    # Retries, timeouts, circuit breaker, degradation, rate limiting
│   ├── DownstreamSimulatorController.cs # Mock downstream services with live SQLite products
│   ├── ChaosController.cs             # Runtime fault injection & resilience event logging
├── Data/
│   ├── AppDbContext.cs                # EF Core SQLite DbContext with seed configurations
├── HealthChecks/
│   ├── DownstreamHealthCheck.cs       # Readiness probe logic
├── Middleware/
│   ├── DeadlinePropagationMiddleware.cs # End-to-end deadline budget enforcement
│   ├── IdempotencyMiddleware.cs       # Persistent idempotency validation
│   ├── LoadSheddingMiddleware.cs      # Concurrency monitoring and load shedding
├── Models/
│   ├── Entities.cs                    # Account, Payment, Product, IdempotencyEntity
│   ├── PaymentModels.cs               # DTOs, ChaosSettings, CatalogItem
├── Services/
│   ├── IDownstreamService.cs          # Downstream client contract
│   ├── DownstreamService.cs           # Implementation with Polly pipeline & fallback
│   ├── IIdempotencyStore.cs           # Store interface
│   ├── EfCoreIdempotencyStore.cs      # SQLite EF Core idempotency store
│   ├── ChaosSimulationState.cs        # In-memory fault injection state & event logs
│   ├── ResiliencePipelines.cs         # Polly v8 resilience pipelines setup
├── Program.cs                         # Application entrypoint & middleware pipeline
├── README.md                          # Architecture, implementation details, and test guide
├── sqlite.db                          # Embedded SQLite database (WAL mode)
└── ReliabilityWebApi.csproj           # Project configuration & NuGet dependencies
```

---

## 🚀 Running the Application

```bash
cd D:/Interview/System-Design/ReliabilityWebApi
dotnet run
```

* **Swagger UI:** `http://localhost:5000/` (or console URL)
* **Live Health Check:** `http://localhost:5000/health/live`
* **Ready Health Check:** `http://localhost:5000/health/ready`

---

## 🧪 Testing Patterns with cURL

### 1. Test Seeded Database Queries
```bash
# Query seeded accounts from SQLite
curl http://localhost:5000/api/payments/accounts

# Query seeded products catalog from SQLite
curl http://localhost:5000/api/simulator/catalog
```

---

### 2. Test Idempotency (Duplicate Prevention & Persistence)
Execute twice with the exact same `Idempotency-Key`:
```bash
curl -X POST http://localhost:5000/api/payments/charge \
  -H "Content-Type: application/json" \
  -H "Idempotency-Key: 9b1deb4d-3b7d-4bad-9bdd-2b0d7b3dcb6d" \
  -d '{"accountId": "ACC-101", "amount": 250.00, "currency": "USD", "description": "Cloud Subscription"}'
```
* **First call:** Deducts balance, persists transaction to `sqlite.db`, returns `TX-...`.
* **Second call:** Returns cached response with header `X-Cache-Lookup: HIT-IDEMPOTENCY` without duplicate deductions or new database records.

---

### 3. Test Retries with Exponential Backoff & Full Jitter
```bash
curl http://localhost:5000/api/resilience/retry-with-backoff
```
The downstream flaky endpoint fails twice with transient 500 errors. Polly catches them, backs off exponentially with randomized jitter, and succeeds on attempt 3.

---

### 4. Test Timeout Policy & Deadline Propagation
```bash
# Simulates 3000ms delay while timeout policy is 1500ms
curl "http://localhost:5000/api/resilience/timeout-demo?delayMs=3000"
```
Returns `504 Gateway Timeout` fast after 1500ms.

---

### 5. Test Circuit Breaker & Chaos Injection
Trip the breaker by simulating a downstream outage:
```bash
# 1. Simulate Outage
curl -X POST http://localhost:5000/api/chaos/configure \
  -H "Content-Type: application/json" \
  -d '{"latencyMs": 0, "failureRatePercent": 0, "simulateDownstreamOutage": true}'

# 2. Fire requests to trip the circuit breaker
curl http://localhost:5000/api/resilience/circuit-breaker-demo

# 3. Check circuit breaker state and logs
curl http://localhost:5000/api/chaos/status

# 4. Check readiness probe (reports Unhealthy because downstream is down)
curl http://localhost:5000/health/ready

# 5. Check liveness probe (still reports Healthy because the service process itself is fine)
curl http://localhost:5000/health/live

# 6. Reset chaos back to normal
curl -X POST http://localhost:5000/api/chaos/reset
```

---

### 6. Test Bulkhead (Vault Concurrency Limit)
```bash
curl -X POST "http://localhost:5000/api/payments/vault-access?holdDurationMs=2000"
```
Fire 3+ parallel requests; excess requests will immediately receive `429 Too Many Requests` (Bulkhead capacity reached).

---

### 7. Test Graceful Degradation (Fallback)
```bash
curl http://localhost:5000/api/resilience/graceful-degradation
```
When live catalog dependencies fail, the API seamlessly returns fallback cached items (`isDegraded: true`).
