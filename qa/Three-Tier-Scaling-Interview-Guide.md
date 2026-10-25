# Senior System Design Interview: How to Scale a 3-Tier Application

## 1. Overview & Problem Statement
- **Role/Level**: Senior / Staff Software Engineer
- **Interview Question**: *"You are given a traditional 3-Tier Web Application (Presentation Tier, Application Tier, Data Tier). It is experiencing severe performance degradation under high traffic. How do you scale it step-by-step? Walk me through your mental model, tier-by-tier bottlenecks, and implementation strategies."*

---

## 2. Stage 1: Clarifying Questions & Scope

A senior engineer avoids generic answers by establishing the specific operational reality:

### Key Clarifying Questions:
1. **Workload Profile**: Is this read-heavy (e-commerce catalog, social feed) or write-heavy (telemetry, financial ledger)?
   - *Interview Assumption*: Read-heavy (85% reads, 15% writes).
2. **Current Architecture State**: Where does state live right now?
   - *Interview Assumption*: Monolithic web servers storing user sessions in local memory (`InProc` sessions) connecting to a single relational database instance (e.g., PostgreSQL or SQL Server).
3. **Bottleneck Symptoms**: Where is the system failing first?
   - *Interview Assumption*: DB connection pool exhaustion (HTTP 500s), high CPU on app servers, and slow p99 latencies (> 3 seconds).
4. **SLAs & RTO/RPO**:
   - Availability: $99.99\%$ (high availability across multiple Availability Zones).
   - Latency: p95 $< 150\text{ ms}$, p99 $< 300\text{ ms}$.

---

## 3. Stage 2: Back-of-the-Envelope Capacity Estimations

Let’s anchor our scaling strategy in quantitative metrics:

- **Active Concurrency**: 50,000 concurrent active users.
- **Average QPS**:
  $$\text{Daily Requests} = 50,000,000 \implies \text{Avg QPS} = \frac{50,000,000}{86,400} \approx 600\text{ QPS}$$
- **Peak Traffic QPS** ($5\times$ surge during campaigns):
  $$\text{Peak QPS} = 600 \times 5 = 3,000\text{ QPS}$$
  - **Peak Read QPS** ($85\%$): $3,000 \times 0.85 = 2,550\text{ QPS}$
  - **Peak Write QPS** ($15\%$): $3,000 \times 0.15 = 450\text{ QPS}$
- **Database Connection Limit Bottleneck**:
  - A single PostgreSQL/SQL Server instance comfortably handles $200\text{–}500$ active concurrent connections before memory and lock contention degrade throughput.
  - At 3,000 peak QPS, without caching or pooling, connection starvation will crash the database.

---

## 4. Stage 3: High-Level Architecture (3-Tier Scaling Flow)

![3-Tier Scaling Architecture](../images/three-tier-scaling-architecture.svg)

### The 3-Tier Scaling Mental Model:
1. **Presentation / Web Tier (Edge & Ingress)**: Move processing as close to the user as possible (DNS, CDN, L4/L7 Load Balancers).
2. **Application / Business Logic Tier (Stateless Compute)**: Eliminate server state, auto-scale stateless compute pods, and offload long-running work to asynchronous queues.
3. **Data Tier (Persistence & Caching)**: Protect the database with multi-layer caching, connection pooling, read replicas, and horizontal partitioning.

---

## 5. Stage 4: Deep Dive — Scaling Tier-by-Tier

### Tier 1: Presentation Tier (Edge & Ingress Scaling)

#### 1. Global CDN & Anycast Routing
- Route user DNS requests via Anycast (Cloudflare / AWS Route 53) to the nearest Point of Presence (PoP).
- **Offload Static Assets**: Store images, JS bundles, and CSS on Blob Storage (S3 / Azure Blob) backed by CDN. This instantly removes $60\%\text{–}70\%$ of HTTP requests from the web servers.

#### 2. Layer 4 vs. Layer 7 Load Balancing
- **L4 (Transport Layer - NLB / IPVS)**: High-throughput packet forwarding based on TCP/UDP IPs and ports. Handles millions of concurrent connections with ultra-low CPU overhead.
- **L7 (Application Layer - YARP / Envoy / NGINX)**: Inspects HTTP headers, cookies, and paths. Handles:
  - TLS/SSL termination (decrypts traffic once at the perimeter to free backend server CPU).
  - Intelligent path routing (e.g., `/api/checkout` $\rightarrow$ Checkout Service; `/api/catalog` $\rightarrow$ Catalog Service).
  - HTTP/2 and HTTP/3 multiplexing back to clients.

---

### Tier 2: Application Tier (Business Logic & Compute Scaling)

#### 1. Enforce 100% Statelessness
- **Problem**: In traditional 3-tier apps, sessions are stored in memory (`Session["User"]`). If a server restarts or a request hits a different server, the session is lost (forcing "sticky sessions" which create traffic hotspots).
- **Solution**:
  - Extract session state into a distributed cache (`Redis`).
  - Prefer stateless cryptographic JWT tokens (validated locally with public keys without database lookups).

#### 2. Horizontal Pod Autoscaling (HPA)
- Containerize the ASP.NET Core / Node.js API into Kubernetes pods.
- Configure HPA based on custom metrics:
  - Scale out when CPU $> 70\%$ OR when HTTP request queue depth exceeds 100 requests per pod.

#### 3. Asynchronous Worker Offloading (Event-Driven Decomposition)
- Never perform heavy compute (PDF invoice generation, credit checks, email dispatch) synchronously inside the HTTP request-response cycle.
- Enqueue jobs to a message broker (RabbitMQ / Apache Kafka / Azure Service Bus) using the **Transactional Outbox Pattern**.
- Autonomous background worker pods scale independently to drain the queue.

---

### Tier 3: Data Tier (Database & Persistence Scaling)

The database is almost always the ultimate bottleneck because compute is easily horizontal, while state is difficult to coordinate. We follow the **Database Scaling Funnel**:

```text
[HTTP Requests: 3,000 QPS]
         |
         v
[L1/L2 Cache: Absorbs 85% of reads] ---> 2,550 QPS absorbed
         |
         v (Remaining 450 writes + cache misses)
[Connection Pooler: PgBouncer / ProxySQL]
         |
    +----+----+
    |         |
    v         v
[Primary DB] [Read Replicas]
 (Writes)      (Reads)
```

#### 1. Caching Strategy (Cache-Aside + Multi-Level)
- **L1 In-Memory Cache**: `IMemoryCache` inside ASP.NET Core for ultra-hot data (5-second TTL).
- **L2 Distributed Cache**: Redis cluster for shared user profiles and product details.
- **Cache Eviction**: LRU (Least Recently Used) with random jitter added to TTL to prevent cache stampedes.

#### 2. Database Connection Pooling
- Opening a TCP connection and performing SSL/auth handshakes to PostgreSQL or SQL Server costs $10\text{–}30\text{ ms}$ and significant memory per connection.
- Deploy a dedicated connection pooler (e.g., **PgBouncer** or **Azure Database Proxy**) between the application pods and the database to multiplex thousands of client connections over a tight pool of 100 persistent physical connections.

#### 3. Read/Write Splitting (CQRS at DB Level)
- Direct all `INSERT/UPDATE/DELETE` operations to the **Primary Database**.
- Direct all `SELECT` queries to multiple **Read Replicas** via asynchronous replication.
- *Trade-off*: Eventual consistency / replication lag ($< 100\text{ ms}$). To prevent user confusion after an update, implement **Read-Your-Own-Writes**: if a user modified their profile, pin their subsequent reads to the Primary for 5 seconds.

#### 4. Partitioning & Sharding
- **Vertical Partitioning**: Split large monolithic tables across databases by domain boundary (`OrdersDB`, `UsersDB`, `InventoryDB`).
- **Horizontal Partitioning (Sharding)**: When dataset exceeds single-node storage ($> 2\text{ TB}$) or write IOPS exceed limits:
  - Hash shard on `TenantId` (multi-tenant SaaS) or `UserId` (consumer apps) using **Consistent Hashing**.

---

## 6. Stage 5: Reliability, Resilience & Enterprise Patterns (.NET Focus)

### 1. Circuit Breakers & Retries with Jitter (`Polly`)
When downstream dependencies or read replicas slow down, blind retries cause **retry storms**.
- **Implementation in .NET**:
  ```csharp
  // Program.cs using Microsoft.Extensions.Http.Resilience
  builder.Services.AddHttpClient("DataServiceClient")
      .AddStandardResilienceHandler(options =>
      {
          options.Retry.MaxRetryAttempts = 3;
          options.Retry.Delay = TimeSpan.FromMilliseconds(200);
          options.Retry.BackoffType = DelayBackoffType.Exponential;
          options.Retry.UseJitter = true; // Prevents synchronized retry spikes

          options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(10);
          options.CircuitBreaker.FailureRatio = 0.5; // Trip if 50% requests fail
          options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(30);
      });
  ```

### 2. Distributed Locking for Concurrency Protection
To prevent double-booking or inventory overselling:
- Use Redis distributed locks (**Redlock** algorithm via `StackExchange.Redis`) with a strict lease timeout.

### 3. Graceful Degradation (Feature Toggles)
If peak traffic overwhelms the system:
- Temporarily disable non-critical features (e.g., product recommendation engines, review widgets) using feature flags (`Microsoft.FeatureManagement`).

---

## 7. Senior Interview Summary Card

| Tier | Bottleneck at Scale | Senior Architectural Solution |
| :--- | :--- | :--- |
| **Presentation** | High bandwidth, SSL overhead, static asset load | Anycast GeoDNS + Global CDN + L4/L7 Load Balancers with TLS Offloading |
| **Application** | In-memory session state, CPU spikes on workers | Stateless JWT/Redis sessions + Kubernetes HPA + Async Message Queues |
| **Data (Reads)** | High read QPS exhausting DB CPU | Multi-level cache (L1 In-Memory + L2 Redis) + Read Replicas |
| **Data (Writes)** | Row locks, connection pool exhaustion | PgBouncer Connection Pooling + Sharding via Consistent Hashing |
| **Resilience** | Cascading timeouts, retry storms | Polly Circuit Breaker + Exponential Backoff with Jitter + Rate Limiting |

