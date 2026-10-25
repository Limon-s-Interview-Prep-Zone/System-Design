# Senior System Design Interview: Scalability (1 to 10M+ Users)

## 1. Overview & Problem Statement
- **Role/Level**: Senior / Staff Software Engineer
- **Question**: Design an end-to-end web architecture that scales gracefully from a single user on day one to 10 million Daily Active Users (DAU). Walk through the milestones, bottlenecks, database evolution, caching tiers, and resilience trade-offs.

---

## 2. Stage 1: Clarifying Questions & Scope

### Functional Requirements
- High-volume read/write e-commerce web platform (catalog browsing, user profiles, carts, order processing).
- Search and filtering across tens of millions of items.

### Non-Functional Requirements & SLAs
- **Availability Target**: $99.99\%$ (Four Nines $\approx 52.6$ minutes downtime/year).
- **Latency Constraints**: p99 read latency $< 100\text{ ms}$, p99 write latency $< 500\text{ ms}$.
- **Consistency**: Eventual consistency for catalog/search and analytics; Strong consistency (ACID) for checkout, payments, and inventory ledger.
- **Geographic Scale**: Global multi-region access with latency-based DNS routing.

---

## 3. Stage 2: Back-of-the-Envelope Capacity Estimations

### 1. Request Throughput (QPS)
- **Daily Active Users (DAU)**: $10,000,000$ ($10\text{M}$).
- **Actions per user/day**: Average $20$ read requests, $2$ write requests.
- **Total Read Requests/day**: $10\text{M} \times 20 = 200,000,000$ requests.
- **Total Write Requests/day**: $10\text{M} \times 2 = 20,000,000$ requests.
- **Seconds per day**: $\approx 100,000$ seconds (exact: 86,400).
- **Average Read QPS**:
  $$\frac{200,000,000}{100,000} = 2,000\text{ QPS}$$
- **Peak Read QPS** ($3\times$ factor):
  $$2,000 \times 3 = 6,000\text{ QPS}$$
- **Peak Write QPS** ($3\times$ factor):
  $$\frac{20,000,000}{100,000} \times 3 = 600\text{ QPS}$$

### 2. Storage Projections (5 Years)
- **Average Write Payload**: $2\text{ KB}$ (metadata, order items, timestamps).
- **Daily Ingress Storage**:
  $$20,000,000 \times 2\text{ KB} = 40\text{ GB/day}$$
- **5-Year Cumulative Storage**:
  $$40\text{ GB} \times 365 \times 5 \approx 73\text{ TB}$$

### 3. Memory & Cache Sizing (80/20 Pareto Rule)
- Cache $20\%$ of daily read requests.
- Daily read data volume: $200\text{M} \times 2\text{ KB} = 400\text{ GB}$.
- **RAM Needed for Cache**:
  $$400\text{ GB} \times 0.20 = 80\text{ GB RAM}$$
  *(A cluster of 3 Redis nodes with 32 GB RAM each handles this with safe headroom).*

---

## 4. Stage 3: High-Level Evolutionary Architecture

![System Scalability Evolution Architecture](../images/scalability-evolutionary-architecture.svg)

### Evolutionary Milestones Breakdown:
1. **Stage 1: 1 - 10K Users (Single Server Monolith)**:
   - Co-located ASP.NET Core Web API and PostgreSQL on a single virtual machine.
   - *Limitation*: Single Point of Failure (SPOF) and shared compute/disk contention.
2. **Stage 2: 10K - 100K Users (Decoupled Web + Read Replicas)**:
   - Stateless web nodes placed behind a Layer 4/7 Load Balancer.
   - Database decoupled into Primary (Writes) and asynchronous Read Replicas.
3. **Stage 3: 100K - 1M Users (CDN, Redis Distributed Cache & Async Queues)**:
   - Cloudflare CDN caches static assets at the network edge.
   - Redis cluster caches $80\%$ of repetitive read queries.
   - RabbitMQ/Kafka decouples asynchronous processing (order fulfillment, email notifications) to background workers.
4. **Stage 4: 10M+ Users (Global Multi-Region & Consistent Hashing Sharding)**:
   - Anycast GeoDNS routes global users to independent regional deployments (US-East, EU-West).
   - Relational database partitioned horizontally via Consistent Hashing on `UserId`.
   - Asynchronous Change Data Capture (Debezium + Kafka) streams updates to Elasticsearch for cross-shard search queries.

---

## 5. Stage 4: Deep Dive — Scaling Tier-by-Tier

### 1. Web & Application Tier (Stateless Scaling)
- **Eliminate Session Affinity**: Store user sessions in Redis or use stateless self-contained JWT tokens.
- **Horizontal Pod Autoscaling (HPA)**: Auto-scale API pods based on custom metrics (HTTP request queue depth, CPU > 70%).
- **.NET Stack Alignment**:
  - Reverse Proxy: `Yarp.ReverseProxy` (L7 routing, SSL termination, health checks).
  - Server Runtime: Kestrel with HTTP/2 and HTTP/3 multiplexing.

### 2. Caching Tier (Preventing Stampedes & Hotkeys)
- **Multi-Level Caching (L1 + L2)**:
  - **L1 In-Memory Cache**: `MemoryCache` with short TTL (e.g., 5 seconds) to absorb celebrity reads locally.
  - **L2 Distributed Cache**: Redis cluster partitioned via consistent hashing.
- **Mitigating Failure Modes**:
  - *Cache Penetration*: Bloom filters at the gateway or caching null results with a 30-second TTL.
  - *Cache Avalanche*: Add random jitter to key TTLs (`TTL = baseTTL + Random(0, 30s)`).
  - *Cache Stampede*: Distributed locks or probabilistic early expiration (XFetch).

### 3. Database Tier (Scaling from Single Instance to Sharding)
- **Read/Write Splitting**: Route `SELECT` queries to Read Replicas (asynchronous replication) and `INSERT/UPDATE/DELETE` to the Primary.
- **Vertical Partitioning**: Separate monolithic tables by domain boundary (`UsersDB`, `OrdersDB`, `CatalogDB`).
- **Horizontal Partitioning (Sharding)**:
  - Shard Key: `UserId` to guarantee user-scoped transactions execute within a single shard.
  - Consistent Hashing with 256 virtual nodes per physical shard to allow seamless rebalancing.
  - Cross-shard queries handled via asynchronous Change Data Capture (Debezium/Kafka) syncing to Elasticsearch/OpenSearch.

---

## 6. Stage 5: Resilience, Bottlenecks & Interview Drills

### Drill 1: Primary Database Failover
- **Problem**: Primary database crashes during peak traffic.
- **Solution**:
  - Automated failover via Patroni/Raft or AWS Aurora Multi-AZ (standby promoted within 30s).
  - Circuit Breakers on the web tier via `Polly` (`Microsoft.Extensions.Http.Resilience`) to fail fast with HTTP 503 rather than exhausting connection pools.
  - Read-your-own-writes consistency: Pin recently updated users to the primary for 5 seconds to bypass replica lag.

### Drill 2: Flash Sale Concurrency
- **Problem**: 100,000 users attempt to purchase 500 limited items concurrently.
- **Solution**:
  - Avoid database row-level locking (`SELECT ... FOR UPDATE`).
  - Use Redis atomic decrement via Lua scripts:
    ```lua
    local current = redis.call('GET', KEYS[1])
    if tonumber(current) > 0 then
        redis.call('DECR', KEYS[1])
        return 1
    else
        return 0
    end
    ```
  - Enqueue successful reservations into RabbitMQ/Kafka; background consumers persist transactions using the Outbox pattern.

### Drill 3: Cascading Failure Protection
- Token Bucket rate limiting via `Microsoft.AspNetCore.RateLimiting`.
- Bulkhead isolation separating thread pools between critical checkout services and search services.

---

## 7. Senior Architectural Summary Card

| Scaling Challenge | Naive Approach | Senior/Staff Production Solution |
| :--- | :--- | :--- |
| **Traffic Spikes** | Vertical Scaling (bigger VM) | Stateless Horizontal Pods + Anycast CDN + Autoscaling |
| **Read Bottleneck** | Querying Master DB directly | L1 MemoryCache + Redis Cluster + Read Replicas |
| **Write Bottleneck** | Single DB Server | Vertical separation $\rightarrow$ Sharding with Consistent Hashing |
| **Complex Search** | SQL `LIKE '%keyword%'` | Asynchronous CDC (Debezium) into Elasticsearch/OpenSearch |
| **Flash Sale Contention** | SQL Row Locks (`FOR UPDATE`) | Redis Atomic Lua decrement + Kafka Queue + Outbox Pattern |
| **Cascading Failures** | Uncontrolled retries | Exponential Backoff with Jitter + Circuit Breaker (`Polly`) |

