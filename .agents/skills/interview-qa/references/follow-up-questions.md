# Interview Follow-Up Questions & Trade-Offs Reference

## Common Deep-Dive Drill Scenarios

### 1. Data Consistency & Replication
- **Scenario**: Primary database fails during an active write.
  - *Response*: Leader election, quorum writes (R + W > N), sync vs async replication trade-offs.

### 2. High-Concurrency & Hot Partitions (Celebrity Problem)
- **Scenario**: 1% of keys receive 90% of traffic (e.g., Justin Bieber / viral tweet).
  - *Response*: Key salting / suffixing, local in-memory L1 cache + Redis L2 cache, write aggregation.

### 3. Distributed Transactions & Microservices
- **Scenario**: Order service completes payment, but inventory reservation times out.
  - *Response*: Saga pattern (Choreography vs Orchestration), compensating transactions, Outbox pattern with idempotent consumers.

### 4. Rate Limiting & Throttling
- **Scenario**: Malicious client bursts 10,000 requests/sec.
  - *Response*: Token Bucket / Sliding Window Log in Redis Lua script, HTTP 429 Too Many Requests, client retry-after header.

