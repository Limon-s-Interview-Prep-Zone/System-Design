# High Availability (HA) Architecture & Disaster Recovery in Distributed Systems

High Availability (HA) is the operational characteristic of a distributed system that ensures an agreed-upon level of operational performance and uptime over a designated time period. Achieving five nines ($99.999\%$) of availability requires designing every layer—DNS, global edge, ingress, compute, inter-service networking, and data storage—to be self-healing, redundant, and resilient against catastrophic failures.

![High Availability Architecture Taxonomy](images/ha-architecture-overview.svg)

---

## 1. Availability Fundamentals, SRE Metrics & Mathematical Modeling

### The Mathematical Definition of Availability
Availability ($A$) represents the ratio of total time a service is operational to the scheduled working duration:

$$\mathcal{A} = \frac{\text{Uptime}}{\text{Uptime} + \text{Downtime}} = \frac{\text{MTBF}}{\text{MTBF} + \text{MTTR}}$$

- **MTBF (Mean Time Between Failures)**: Average operational time between service disruptions.
- **MTTR (Mean Time to Repair / Resolve)**: Average duration required to detect, isolate, and restore a failed system component.
- **MTTD (Mean Time to Detect)**: Time elapsed from when an incident starts until engineers or automated alerting detect it.

### The Nine's of Availability (Uptime vs. Downtime Reference)

| Availability Tier | Annual Downtime | Monthly Downtime | Weekly Downtime | Daily Downtime | Industry Example |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **90.0% (One 9)** | 36.53 days | 72.00 hours | 16.80 hours | 2.40 hours | Experimental batch jobs, internal dev tools |
| **99.0% (Two 9s)** | 3.65 days | 7.20 hours | 1.68 hours | 14.40 minutes | Standard non-critical internal apps |
| **99.9% (Three 9s)** | 8.77 hours | 43.83 minutes | 10.08 minutes | 1.44 minutes | Standard SaaS, consumer web apps |
| **99.99% (Four 9s)** | 52.60 minutes | 4.38 minutes | 1.01 minutes | 8.64 seconds | E-commerce checkout, banking web portals |
| **99.999% (Five 9s)** | 5.26 minutes | 26.30 seconds | 6.05 seconds | 864.0 milliseconds | Telecom core, global payments (Visa/Stripe) |
| **99.9999% (Six 9s)** | 31.56 seconds | 2.63 seconds | 604.8 milliseconds | 86.4 milliseconds | High-frequency trading, air traffic control |

---

### Component Availability: Sequence (Series) vs. Parallel

Distributed systems combine multiple components. The total availability depends strictly on whether the components fail in series or failover in parallel.

#### A. Components in Series (Sequential Dependency)
If Component A calls Component B, and Component B calls Component C, the overall system functions only if **all** components are simultaneously available. Every added sequential dependency reduces total availability:

$$\mathcal{A}_{\text{series}} = \prod_{i=1}^{n} \mathcal{A}_i = \mathcal{A}_1 \times \mathcal{A}_2 \times \dots \times \mathcal{A}_n$$

*Example*: If an API Gateway ($99.9\%$), an Order Service ($99.9\%$), and a Database ($99.9\%$) are in series:

$$\mathcal{A}_{\text{series}} = 0.999 \times 0.999 \times 0.999 = 0.997 \quad (99.70\% \text{ availability; downtime jumps from } 8.77 \text{ hrs to } 26.28 \text{ hrs/year!})$$

#### B. Components in Parallel (Redundant / Clustered)
If a service is deployed across redundant, load-balanced instances in parallel, the service fails only if **all** replicas fail simultaneously:

$$\mathcal{A}_{\text{parallel}} = 1 - \prod_{i=1}^{n} (1 - \mathcal{A}_i) = 1 - (1 - \mathcal{A}_1)(1 - \mathcal{A}_2)\dots(1 - \mathcal{A}_n)$$

*Example*: If two independent web servers each have $99.0\%$ availability ($0.99$), running them in parallel yields:

$$\mathcal{A}_{\text{parallel}} = 1 - (1 - 0.99)(1 - 0.99) = 1 - (0.01 \times 0.01) = 1 - 0.0001 = 0.9999 \quad (99.99\% \text{ Four 9s!})$$

---

### Availability vs. Reliability vs. Fault Tolerance

| Characteristic | Availability | Reliability | Fault Tolerance |
| :--- | :--- | :--- | :--- |
| **Core Question** | *"Is the system ready to serve requests right now?"* | *"Can the system run without any failure over a specified duration $T$?"* | *"Can the system sustain catastrophic hardware failure with zero user interruption?"* |
| **Tolerance to Failure** | Recovers quickly after failure (low MTTR keeps availability high). | Avoids failures altogether (high MTBF). | Transparently masks hardware failure via synchronized physical redundancy. |
| **Cost Profile** | Moderate (Standard cloud redundancy & auto-scaling). | High (Rigorous testing, high-grade components). | **Extremely High** (Duplicated hardware, lockstep CPUs, 2N power). |

---

### SRE Reliability Governance: SLI, SLO, SLA & Error Budgets

In modern cloud platforms, availability is tracked and governed by Site Reliability Engineering (SRE) frameworks:

1. **SLI (Service Level Indicator)**: A quantifiable metric measured in production.
   $$\text{Availability SLI} = \frac{\text{Count of Successful HTTP Requests (Status } < 500\text{)}}{\text{Total Valid HTTP Requests}} \times 100$$
2. **SLO (Service Level Objective)**: The internal target reliability agreed upon by the engineering team (e.g., $99.95\%$ success over a rolling 30-day window).
3. **SLA (Service Level Agreement)**: The external legal contract with customers detailing financial refunds or service credits if availability drops below a threshold (e.g., $< 99.9\%$).
4. **Error Budget**: The allowable downtime or failure rate during an SLO period ($100\% - \text{SLO}$):
   $$\text{Error Budget} = 1 - 0.9995 = 0.05\% \quad (\approx 21.6 \text{ minutes of downtime per month})$$
   - *Burn Rate Policy*: If feature releases burn $> 50\%$ of the monthly error budget in 48 hours, all feature deployments are automatically frozen to prioritize reliability engineering.

---

## 2. Disaster Recovery (DR) Objectives & Strategies

Catastrophic events (data center power failure, undersea cable severing, earthquake, or ransomware) require a formal Disaster Recovery plan.

![Disaster Recovery Strategies RTO vs RPO](images/dr-strategies-rto-rpo.svg)

### Key Metrics: RTO vs. RPO
- **RTO (Recovery Time Objective)**: The maximum tolerable duration of system downtime. Answers: *"How long can the business afford to be offline before restoring operations?"*
- **RPO (Recovery Point Objective)**: The maximum acceptable age of data that must be recovered. Answers: *"How much data loss (measured in time) can the organization tolerate?"*

### The 4 Disaster Recovery Tiers

| DR Strategy Tier | RTO (Downtime) | RPO (Data Loss) | Infrastructure Cost | Operational Complexity | Target Workload |
| :--- | :--- | :--- | :--- | :--- | :--- |
| **1. Backup & Restore** | $24+$ Hours | Hours to Days | **$** (Very Low) | Low (Periodic backup scripts) | Internal tooling, archival data, batch systems |
| **2. Pilot Light** | $10 - 60$ Minutes | Seconds to Minutes | **$$** (Low to Moderate) | Moderate (IaC automation to spin up VMs) | Core business backends, non-critical SaaS |
| **3. Warm Standby** | $1 - 5$ Minutes | Seconds | **$$$** (Moderate to High) | High (Continuous sync, scaled-down cluster) | E-commerce checkout, consumer banking apps |
| **4. Multi-Region Active-Active** | **$\approx 0$ Seconds** | **$\approx 0$ (Near Zero)** | **$$$$$** (Extremely High) | Extreme (CRDTs, cross-region replication) | Tier-0 financial settlement, Visa, Netflix |

---

## 3. Failover Topologies & Consensus Models

When an active primary instance or whole region fails, the system must redirect traffic and promote standby resources.

![Failover Topologies and Consensus Architecture](images/failover-topologies.svg)

### A. Active-Passive Failover
- **Hot Standby**: Standby instance is powered on, fully synchronized with live data, and runs idle. Failover takes seconds via DNS or virtual IP switch.
- **Warm Standby**: Standby runs minimal capacity (e.g., 2 pods instead of 50). Failover takes minutes while auto-scalers spin up remaining pods.
- **Cold Standby**: Standby resources are shut down or unprovisioned. Failover requires provisioning infrastructure from Terraform/AMIs and restoring data.

### B. Active-Active Clustering
Both nodes/regions simultaneously serve live production traffic.
- **Advantages**: $100\%$ resource utilization (no idle servers) and near-zero failover downtime ($RTO \approx 0$).
- **The $50\%$ Capacity Headroom Rule**: In a 2-region active-active cluster, each region must run at $\le 50\%$ CPU/memory utilization under normal load. If Region A fails, Region B must instantly absorb $100\%$ of global traffic without crashing from CPU saturation.

### C. Quorum Consensus & Split-Brain Prevention
When network partitions divide a cluster into two disconnected segments, both sides might assume the other has crashed. If both declare themselves the Primary and write to storage, data corruption and catastrophic data divergence (**Split-Brain**) occurs.

#### 1. Quorum Calculation Rule
A cluster can safely accept writes only if a partition holds a strict mathematical majority (**Quorum**):

$$\text{Quorum} = \left\lfloor \frac{N}{2} \right\rfloor + 1$$

| Total Cluster Nodes ($N$) | Quorum Required ($Q$) | Maximum Tolerable Node Failures |
| :---: | :---: | :---: |
| **3** | **2** | 1 node |
| **5** | **3** | 2 nodes |
| **7** | **4** | 3 nodes |

*Rule*: Clusters must always deploy an **odd number of voting nodes** ($3, 5, 7$) across independent failure domains.

#### 2. Fencing Tokens & STONITH
- **Fencing Tokens**: A monotonically increasing transaction counter issued by the consensus leader (e.g., Zookeeper or etcd). When a partitioned former leader attempts to write with an older generation token, the database rejects the write.
- **STONITH ("Shoot The Other Node In The Head")**: A hardware/IPMI fence where surviving nodes remotely cut power to unresponsive nodes to guarantee they cannot write corrupt data.

---

## 4. Health Probing, Failure Detection & Telemetry

Automated self-healing requires reliable health detection that differentiates between slow initialization, software deadlocks, and saturated dependencies.

![Health Probing Architecture](images/health-checks-probes.svg)

### The 3 Kubernetes & Cloud Probing Paradigms

| Probe Type | Endpoint | Evaluated Conditions | Action on Failure |
| :--- | :--- | :--- | :--- |
| **Startup Probe** | `/health/startup` | Database migrations, cache pre-warming, JIT compilation. | Disables Liveness & Readiness. Kills container only if initialization exceeds timeout limit. |
| **Liveness Probe** | `/health/live` | Internal application thread responsiveness, deadlock detection. | **Restarts the container** via container runtime (`SIGTERM` ➔ `SIGKILL`). |
| **Readiness Probe** | `/health/ready` | Downstream connectivity (Database, Redis, Message Broker). | **Removes container IP from Load Balancer** endpoints; does **NOT** restart the container! |

> [!CAUTION]
> **The Cascading Death Spiral Anti-Pattern**:
> **Never** check external dependencies (such as PostgreSQL or Redis) inside a **Liveness Probe**!
> If your database suffers a temporary 5-second connection hiccup, all 50 application pods running the liveness probe will fail simultaneously. Kubernetes will reboot all 50 pods at once, wiping their in-memory caches, flooding the database with 50 connection requests during boot, and turning a minor blip into complete system collapse.

---

## 5. Fault Isolation, Resiliency & Graceful Degradation

High availability is maintained not by hoping failures won't happen, but by containing their blast radius.

### A. Bulkhead Pattern
Named after the watertight compartments in ships: if one section floods, the hull stays afloat. In software, thread pools, memory pools, and database connection pools are separated by business function.
- *Example*: Saturated reporting queries execute on a dedicated connection pool, preventing them from starving the checkout transaction pool.

### B. Circuit Breaker & Exponential Backoff with Full Jitter
- **Circuit Breaker**: When downstream error rates exceed $50\%$, the breaker transitions from `Closed` to `Open`, failing fast immediately and allowing downstream servers to recover.
- **Full Jitter**: Randomizes retry delays across workers to prevent **Thundering Herd** spikes against recovering services:
  $$T_{\text{sleep}} = \text{random}(0, \, \min(T_{\text{max}}, \, T_{\text{base}} \times 2^{\text{attempt}}))$$

### C. Load Shedding & Request Prioritization
When CPU utilization exceeds $85\%$, the system drops incoming requests before queuing them:
1. **Tier-1 (Critical)**: Checkout, payment processing, authentication ➔ Processed normally.
2. **Tier-2 (Non-Critical)**: Product reviews, recommendations, analytics ➔ Dropped with `HTTP 429 / 503` or served from stale cache.

---

## 6. Data Tier High Availability & Replication Topologies

The compute tier is stateless and easy to scale; the data tier holds state and governs the actual availability limit of your architecture.

### Replication Models Comparison

| Replication Topology | Consistency | Availability (CAP) | RPO (Data Loss) | Primary Use Cases |
| :--- | :--- | :--- | :--- | :--- |
| **Single-Leader (Synchronous)** | Strong Consistency | Lower (Write blocked if replica down) | **$RPO = 0$** | Financial ledgers, ACID relational DBs |
| **Single-Leader (Asynchronous)** | Eventual Consistency | High (Writes complete on Primary) | Seconds of loss | Read-heavy web applications, PostgreSQL read replicas |
| **Multi-Leader (Dual Primary)** | Eventual Consistency | Very High (Local writes in every region) | Conflict window | Multi-region collaborative apps, Git, CMS |
| **Leaderless (Quorum $R+W > N$)** | Tunable Consistency | Maximum (Any node accepts writes) | Tunable | Amazon DynamoDB, Apache Cassandra |

### Tunable Quorum in Leaderless Systems
In Dynamo/Cassandra architectures:
- $N$ = Number of replicas stored.
- $W$ = Number of replicas that must acknowledge a write before success.
- $R$ = Number of replicas queried during a read operation.

$$\text{Strong Consistency Rule:} \quad \mathbf{W + R > N}$$

*Example*: If $N = 3$, setting $W = 2$ and $R = 2$ guarantees that at least one node in the read set has the latest write acknowledgment ($2 + 2 = 4 > 3$), ensuring strong consistency even if 1 node dies!

---

## 7. Global Routing, DNS & Ingress Availability

Single points of failure at the DNS or IP layer will render multi-region architectures completely unreachable.

### Ingress Technologies
1. **Anycast BGP Routing**: Multiple worldwide data centers advertise the exact same IP address via Border Gateway Protocol (BGP). User traffic is automatically routed to the topologically closest healthy Point of Presence (PoP). If a PoP goes down, internet routers automatically withdraw the route in seconds.
2. **Geo-DNS & Health-Checked Routing (AWS Route 53)**: Periodically probes backend endpoints. When an origin fails health checks, DNS queries return the IP of the healthy secondary region.
3. **Multi-CDN Architecture**: Distributes traffic across two independent CDN providers (e.g., Cloudflare and Fastly). If one CDN experiences an outage, DNS dynamically shifts traffic to the other.

---

## 8. Zero-Downtime Deployment & Verification

Planned maintenance and continuous deployment must never degrade user availability.

### Deployment Topologies
- **Rolling Update**: Replaces pods one by one. Low infrastructure cost, but old and new versions run simultaneously.
- **Blue-Green Deployment**: Provisions a full duplicate environment (Green) running the new version alongside production (Blue). Once tested, the router switches traffic instantly ($RTO = 0$). Rollback is instantaneous.
- **Canary Release**: Routes $2\%$ of production traffic to the new version. Automated telemetry monitors error rates and latency before gradually expanding to $10\%$, $50\%$, and $100\%$.

### Zero-Downtime Database Migrations: Expand-Contract Pattern
Renaming or removing a database column directly breaks running application instances. The **Expand-Contract (Parallel Run)** pattern solves this:
1. **Expand**: Add the new column alongside the old one. Keep both nullable.
2. **Dual Write**: Application writes to both the old and new columns.
3. **Backfill**: Asynchronously migrate historical rows from old column to new column.
4. **Read Switch**: Switch application reads to the new column.
5. **Contract**: Stop writing to the old column and safely drop it in a future release.

---

## 9. Architectural Standard (.NET / C# Focus)

### Benefits:
- **Native Diagnostic Endpoints**: Built-in ASP.NET Core health checks interface seamlessly with Kubernetes, Azure App Service, and AWS ALB.
- **Standardized Resilience**: .NET 8/9 provides `Microsoft.Extensions.Http.Resilience` and `Polly v8` for declarative, non-allocating resilience pipelines.

### Drawbacks & Trade-offs:
- **Probe Overhead**: Aggressive readiness probes with heavy SQL queries can overwhelm database connection pools. Use lightweight `SELECT 1` queries and caching.
- **Memory Consumption**: Bulkhead thread pools must be sized carefully to avoid thread exhaustion.

### Target Technology & NuGet Packages for .NET / C#:
- `Microsoft.Extensions.Diagnostics.HealthChecks` (Native health checking engine)
- `AspNetCore.HealthChecks.UI.Client` (Standardized JSON output formatter for Kubernetes & UI dashboards)
- `AspNetCore.HealthChecks.NpgSql` / `AspNetCore.HealthChecks.SqlServer` (Ready-made DB probes)
- `Microsoft.Extensions.Http.Resilience` (Official .NET resilience handler incorporating Polly)

---

### Production-Grade C# Example 1: Enterprise ASP.NET Core Health Checks Pipeline

This implementation provides separate `/health/live` (for Kubernetes liveness) and `/health/ready` (for load balancer readiness) endpoints with custom diagnostic details.

```csharp
// Program.cs - High Availability Health Probing in .NET 8/9
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

// Register Health Checks
builder.Services.AddHealthChecks()
    // 1. Liveness Probe Check (Only checks local process health; NO external dependencies)
    .AddCheck("self", () => HealthCheckResult.Healthy("Process is alive"), tags: new[] { "live" })
    
    // 2. Readiness Probe Checks (Validates critical external dependencies before accepting traffic)
    .AddCheck("primary_database", () =>
    {
        // Replace with actual DB connection check (e.g., SELECT 1)
        bool dbConnected = true; 
        return dbConnected 
            ? HealthCheckResult.Healthy("Database is responsive.")
            : HealthCheckResult.Unhealthy("Database connection timeout.");
    }, tags: new[] { "ready" })
    .AddCheck("distributed_cache", () =>
    {
        bool redisConnected = true;
        return redisConnected
            ? HealthCheckResult.Healthy("Redis cluster is reachable.")
            : HealthCheckResult.Degraded("Redis latency high; running in degraded cache mode.");
    }, tags: new[] { "ready" });

var app = builder.Build();

// Endpoint 1: Kubelet Liveness Probe (/health/live)
// Restarts container if process deadlocks. Filters only 'live' tagged checks.
app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

// Endpoint 2: Load Balancer Readiness Probe (/health/ready)
// Removes pod from ingress endpoint pool if DB/Redis fails. Does NOT restart container.
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("ready"),
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.MapGet("/", () => "API Gateway is operational.");

app.Run();
```

---

### Production-Grade C# Example 2: Resilient HTTP Pipeline with Circuit Breaker & Fallback

```csharp
// Program.cs - Polly Resilience Pipeline for High Availability Inter-Service Calls
using Microsoft.Extensions.Http.Resilience;
using Polly;
using Polly.CircuitBreaker;

var builder = WebApplication.CreateBuilder(args);

// Register an HttpClient with standard resilience handler (Retry, Circuit Breaker, Timeout)
builder.Services.AddHttpClient("PaymentService", client =>
{
    client.BaseAddress = new Uri("https://payment-service.internal:5001");
})
.AddStandardResilienceHandler(options =>
{
    // 1. Circuit Breaker Configuration
    options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(30);
    options.CircuitBreaker.FailureRatio = 0.5; // Trip breaker if 50% of calls fail
    options.CircuitBreaker.MinimumThroughput = 20; // Require at least 20 calls in window
    options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(15); // Open state duration

    // 2. Exponential Backoff with Jitter
    options.Retry.MaxRetryAttempts = 3;
    options.Retry.BackoffType = DelayBackoffType.Exponential;
    options.Retry.UseJitter = true;
    options.Retry.Delay = TimeSpan.FromMilliseconds(200);

    // 3. Strict Request Timeout
    options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(5);
});

var app = builder.Build();

app.MapPost("/checkout", async (IHttpClientFactory factory) =>
{
    var client = factory.CreateClient("PaymentService");
    try
    {
        var response = await client.PostAsync("/api/pay", null);
        response.EnsureSuccessStatusCode();
        return Results.Ok(new { status = "Payment Processed" });
    }
    catch (BrokenCircuitException)
    {
        // Graceful Degradation: Circuit is OPEN; fail immediately without hitting downed service
        return Results.Json(new 
        { 
            status = "Queued", 
            message = "Payment gateway temporarily degraded. Order placed in asynchronous processing queue." 
        }, statusCode: 202);
    }
});

app.Run();
```

---

## 10. System Design Interview Decision Framework for High Availability

When asked to design a highly available architecture in an interview, structure your response using this sequential decision framework:

```
Availability Requirement Analysis
 ├── What is the target SLA / SLO?
 │    ├── 99.9% (Three 9s: ~8.7 hrs downtime/year) ──► Single Region, Multi-AZ, Load Balancer, Standard Probes
 │    ├── 99.99% (Four 9s: ~52 min downtime/year) ──► Multi-AZ Cluster, Auto-failover DB, Warm Standby DR
 │    └── 99.999% (Five 9s: ~5 min downtime/year) ──► Multi-Region Active-Active, GSLB, Anycast, Quorum Storage
 │
 ├── How to protect Compute Tier from failure?
 │    ├── Container crash/deadlock? ──► Liveness Probe (process restart)
 │    ├── High load / dependency lag? ──► Readiness Probe (traffic shedding without restart)
 │    └── Zero-Downtime deployments? ──► Canary Releases + Expand-Contract Database Migrations
 │
 ├── How to protect Data Tier from data loss?
 │    ├── RPO = 0 mandatory? ──► Synchronous Replication across AZs + Raft/Paxos consensus
 │    ├── Multi-Region latency too high? ──► Local Async Replicas + Outbox Pattern + Eventual Consistency
 │    └── Network partition risk? ──► Quorum (⌊N/2⌋ + 1) + Fencing Tokens (prevents Split-Brain)
 │
 └── How to handle downstream third-party outages?
      ├── Transient network blip? ──► Exponential Backoff with Full Jitter
      └── Persistent failure? ──► Circuit Breaker with Graceful Degradation / Stale Cache Fallback
```
