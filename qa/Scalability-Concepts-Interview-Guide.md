# Senior System Design Interview: Scalability Concepts & Patterns

> **Interviewer Prompt:**  
> *"Walk me through your complete mental model for scalability. What are the foundational patterns, and how do you decide which lever to pull first?"*

---

## 1. Stage 1: Clarifying Questions & Scope

### What Senior Engineers Ask Before Answering:
1. **Workload Type**: Read-heavy (content platform, social feed)? Write-heavy (IoT telemetry, financial ledger)? Mixed (e-commerce)?
2. **Current Scale & Pain Point**: What is the existing DAU/QPS? Where is the system failing first — CPU, memory, disk IOPS, network, or connection pool?
3. **Consistency Model**: Does every read need the absolute latest data (Strong Consistency / ACID), or is a few seconds of staleness acceptable (Eventual Consistency)?
4. **Latency Budget**: What is the acceptable p99 latency? ($< 100\text{ ms}$ for interactive UIs, $< 1\text{ s}$ for batch analytics).
5. **Availability SLA**: $99.9\%$ (8.7 hours downtime/year) vs. $99.99\%$ (52 minutes/year) — this fundamentally changes redundancy requirements.

---

## 2. Stage 2: Back-of-the-Envelope Capacity Estimation

### The 4-Number Estimation Toolkit

Every scalability interview question reduces to calculating 4 numbers:

### Number 1: Traffic Throughput (QPS)
$$\text{Average QPS} = \frac{\text{Daily Requests}}{100,000 \text{ seconds (rounded)}}$$
$$\text{Peak QPS} = \text{Average QPS} \times (2\text{ to }5)$$

**Worked Example** (100M DAU social platform, 10 actions/user/day):
$$\text{Daily Requests} = 100\text{M} \times 10 = 1,000,000,000 = 10^9$$
$$\text{Average QPS} = \frac{10^9}{10^5} = 10,000\text{ QPS}$$
$$\text{Peak QPS} = 10,000 \times 3 = \mathbf{30,000\text{ QPS}}$$

### Number 2: Storage Growth (5 Years)
$$\text{Daily Storage} = \text{Daily Writes} \times \text{Avg Payload Size}$$
$$\text{5-Year Storage} = \text{Daily Storage} \times 365 \times 5 \approx \text{Daily} \times 2,000$$

**Worked Example** (20% writes = 200M writes/day, 500 bytes each):
$$\text{Daily} = 200\text{M} \times 500\text{ B} = 100\text{ GB/day}$$
$$\text{5-Year} = 100\text{ GB} \times 2,000 = \mathbf{200\text{ TB}}$$

### Number 3: Network Bandwidth
$$\text{Egress (Read BW)} = \text{Read QPS} \times \text{Response Payload Size}$$

**Worked Example** (24,000 Read QPS × 10 KB response):
$$\text{Egress} = 24,000 \times 10\text{ KB} = 240,000\text{ KB/s} = \mathbf{240\text{ MB/s}} \approx \mathbf{1.9\text{ Gbps}}$$

> **Decision**: A single 1 Gbps NIC is insufficient. Need 10 Gbps NICs or distribute across multiple servers behind a load balancer.

### Number 4: Cache Memory (80/20 Pareto Rule)
$$\text{Cache RAM} = \text{Daily Read Volume} \times 0.20$$

**Worked Example** (800M reads/day × 2 KB each):
$$\text{Daily Read Volume} = 800\text{M} \times 2\text{ KB} = 1.6\text{ TB}$$
$$\text{Cache RAM} = 1.6\text{ TB} \times 0.20 = \mathbf{320\text{ GB RAM}}$$

> **Decision**: Deploy a 10-node Redis cluster (32 GB RAM each) with replicas for fault tolerance.

---

## 3. Stage 3: High-Level Architecture — The Scalability Patterns Map

![Scalability Patterns Mental Model](../images/scalability-patterns-mental-model.svg)

### The 5 Core Scalability Pillars

| # | Pillar | What It Solves | Key Technologies (.NET / C#) |
| :---: | :--- | :--- | :--- |
| 1 | **Vertical vs. Horizontal Scaling** | Resource capacity ceiling | Kubernetes HPA, VM Scale Sets |
| 2 | **Stateless Compute Tier** | Server affinity & session hotspots | JWT Tokens, Redis Session Store, `Yarp.ReverseProxy` |
| 3 | **Multi-Level Caching** | Read amplification & DB load | `IMemoryCache` (L1), `StackExchange.Redis` (L2), CDN (L0) |
| 4 | **Async Event Decoupling** | Long-running task blocking & tight coupling | RabbitMQ, Kafka, `MassTransit`, Transactional Outbox |
| 5 | **Database Scaling Funnel** | Write throughput & storage limits | PgBouncer, Read Replicas, Consistent Hashing Sharding |

---

## 4. Stage 4: Deep Dive — Each Pillar Explained

### Pillar 1: Vertical Scaling vs. Horizontal Scaling

#### Vertical Scaling (Scale Up)
- Add more CPU, RAM, or faster disks to a single server.
- **Benefits**:
  - Zero code changes. Just resize the VM.
  - No distributed coordination overhead.
- **Drawbacks & Trade-offs**:
  - Hardware ceiling ($96$ cores / $24\text{ TB}$ RAM is the practical cloud maximum).
  - Single Point of Failure. If the machine dies, the entire system dies.
  - Cost grows exponentially (a server with $2\times$ CPU costs $3\text{–}4\times$ the price).

#### Horizontal Scaling (Scale Out)
- Add more servers (nodes/pods) behind a load balancer.
- **Benefits**:
  - No hardware ceiling. Add nodes linearly with demand.
  - Fault tolerant. One node failing does not crash the system.
  - Cost-efficient (commodity hardware).
- **Drawbacks & Trade-offs**:
  - Application **must be stateless** (sessions cannot live in server memory).
  - Introduces distributed system complexity (network partitions, consistency challenges).
- **Target Technology (.NET)**:
  - Kubernetes Horizontal Pod Autoscaler (HPA).
  - `Yarp.ReverseProxy` for L7 intelligent routing.

> **Senior Rule of Thumb**: Start vertical for simplicity. Switch to horizontal when you hit the cost-efficiency crossover (typically around $4\text{–}8$ cores per instance).

---

### Pillar 2: Stateless Application Tier

Why must the compute tier be stateless for horizontal scaling?

| Stateful Server (Anti-Pattern) | Stateless Server (Production Pattern) |
| :--- | :--- |
| Session stored in local memory (`InProc`) | Session stored in Redis or encoded in JWT |
| Requires sticky sessions (L7 affinity) | Any server can handle any request |
| Server restart = all sessions lost | No data loss on pod restart |
| Cannot auto-scale safely | HPA scales pods freely |

**Implementation in .NET**:
```csharp
// Externalize sessions to Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = "redis-cluster:6379";
    options.InstanceName = "SessionStore:";
});
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.IsEssential = true;
});
```

---

### Pillar 3: Multi-Level Caching Strategy

The cache sits between the application and the database, absorbing repetitive reads:

```text
Request Flow:
  Client → CDN (L0) → App Pod (L1: IMemoryCache) → Redis (L2) → Database (Cold Path)

Hit Rates (typical):
  L0 CDN:  60-70% of static asset requests never reach your servers
  L1 Mem:  20-30% of dynamic reads served from local RAM (zero network hop)
  L2 Redis: 40-50% of remaining reads served from distributed cache
  DB:       Only 5-15% of original requests actually hit the database
```

#### Cache Failure Modes & Mitigations

| Failure Mode | What Happens | Mitigation |
| :--- | :--- | :--- |
| **Cache Penetration** | Queries for non-existent keys bypass cache, hammer DB | Bloom filters at gateway; Cache null results with 30s TTL |
| **Cache Avalanche** | Many keys expire simultaneously, causing mass DB load | Add random jitter to TTL: `TTL = baseTTL + Random(0, 30s)` |
| **Cache Stampede** | Hot key expires, 1000 threads simultaneously query DB | Distributed lock (Redlock) or probabilistic early expiration (XFetch) |
| **Hot Key Problem** | Celebrity/viral content overloads a single Redis shard | Key salting: append random suffix, local L1 cache absorbs reads |

---

### Pillar 4: Asynchronous Event Decoupling

#### The Problem with Synchronous Processing:
When a user places an order, a synchronous system does this inside the HTTP request:
1. Validate order → 2. Charge payment → 3. Update inventory → 4. Send email → 5. Generate PDF invoice → 6. Return HTTP 200

If step 4 (email) takes 3 seconds, the user waits 3+ seconds for the entire chain. Under high load, thread pools exhaust and the entire API becomes unresponsive.

#### The Async Solution (Event-Driven Architecture):
1. API receives order → writes to DB + publishes `OrderPlacedEvent` to message broker → returns HTTP 202 Accepted immediately.
2. Independent worker consumers process email, PDF, analytics asynchronously at their own pace.

#### Transactional Outbox Pattern (Reliability Guarantee):
- **Problem**: If the app writes to the DB but crashes *before* publishing the event to Kafka, the event is lost.
- **Solution**: Write the event to an `OutboxMessages` table inside the same database transaction. A relay service polls the outbox and publishes to the broker.

**Implementation in .NET** (`MassTransit`):
```csharp
// Startup configuration
builder.Services.AddMassTransit(x =>
{
    x.AddEntityFrameworkOutbox<AppDbContext>(o =>
    {
        o.UsePostgres();          // Outbox stored in same DB
        o.UseBusOutbox();         // Relay publishes to broker
        o.QueryDelay = TimeSpan.FromSeconds(1);
    });
    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host("rabbitmq://localhost");
        cfg.ConfigureEndpoints(context);
    });
});
```

---

### Pillar 5: Database Scaling Funnel

The database is the hardest component to scale because it holds mutable state. Follow this sequential funnel — never jump to sharding before exhausting simpler options:

| Step | When to Apply | What It Does | .NET Technology |
| :---: | :--- | :--- | :--- |
| **1. Connection Pooling** | Always (Day 1) | Multiplexes 3,000 app connections over 100 physical DB connections | PgBouncer, Azure DB Proxy, ADO.NET `MaxPoolSize` |
| **2. Read/Write Splitting** | Read QPS > 2,000 | Routes reads to N replicas, writes to primary | EF Core `UseReadReplica()`, Npgsql connection routing |
| **3. Vertical Partitioning** | Schema > 100 tables or microservice boundaries | Splits monolithic DB into domain databases (UsersDB, OrdersDB) | Separate `DbContext` per domain |
| **4. Horizontal Sharding** | Single-node disk > 2 TB or write IOPS saturated | Hash-partitions rows across N database nodes | Consistent Hashing, Citus (PostgreSQL), Vitess (MySQL) |
| **5. CDC + Search Index** | Complex cross-shard queries needed | Streams DB changes to Elasticsearch via Debezium/Kafka | Debezium CDC connector, `NEST` (.NET ES client) |

---

## 5. Stage 5: Resilience, Bottlenecks & Follow-Up Drills

### Drill 1: *"What if your Redis cache cluster goes down entirely?"*
- **Immediate**: Circuit breaker trips. App falls back to reading directly from Read Replicas (degraded latency, not outage).
- **Protection**: Rate limit the fallback path to prevent DB overload. Return stale data from L1 in-memory cache where possible.
- **Recovery**: Redis Sentinel or Redis Cluster with automatic failover promotes a replica to primary within 10–30 seconds.

### Drill 2: *"How do you handle a thundering herd after a cache key expires?"*
- **Distributed Lock**: Only one thread acquires the lock, queries the DB, and repopulates the cache. All other threads wait and then read the fresh cache.
- **Probabilistic Early Expiration (XFetch)**: Each read probabilistically refreshes the cache before TTL actually expires. The probability increases as TTL approaches zero:
  $$P(\text{recompute}) = \delta \times \ln(\text{random()}) + (\text{TTL} - \text{now}) \leq 0$$

### Drill 3: *"Your database primary fails during a flash sale. What happens?"*
- **Automated Failover**: Patroni (PostgreSQL) or Always On AG (SQL Server) promotes a synchronous standby within 30 seconds.
- **Circuit Breaker on Writes**: `Polly` opens the circuit after 5 consecutive failures, returning HTTP 503 with `Retry-After` header.
- **Idempotent Retries**: Clients retry with the same `Idempotency-Key` header. The server deduplicates using a Redis key or DB unique constraint.

---

## 6. Senior Summary: Scalability Decision Matrix

| Question to Ask | If Answer Is... | Scaling Action |
| :--- | :--- | :--- |
| Is the app stateful (in-memory sessions)? | Yes | Extract sessions to Redis / JWT tokens |
| Are > 60% of requests for static content? | Yes | Deploy CDN + Blob Storage |
| Is read QPS > single DB capacity? | Yes | Add L1/L2 caching + Read Replicas |
| Is write QPS > single DB capacity? | Yes | Connection Pooling → Sharding |
| Are long tasks (email, PDF) done in HTTP cycle? | Yes | Decouple with Message Broker + Outbox |
| Is dataset > 2 TB or IOPS maxed? | Yes | Horizontal Sharding (Consistent Hashing) |
| Do users span multiple continents? | Yes | Multi-Region with GeoDNS + Regional Stacks |

