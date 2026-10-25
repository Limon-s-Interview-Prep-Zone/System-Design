# Senior System Design Interview Framework Reference

## 1. Capacity Estimation Quick Reference
- **Seconds per day**: $\approx 86,400 \approx 10^5 \text{ seconds}$.
- **QPS**:
  $$\text{Daily Requests} / 100,000$$
- **Peak QPS**: Typically $2\times$ to $5\times$ average QPS.
- **Storage Growth**:
  $$\text{Daily Writes} \times \text{Avg Payload Size} \times 365 \times 5 \text{ years}$$
- **Cache Memory Rule of Thumb**: Cache $20\%$ of daily read volume (80/20 Pareto rule).

---

## 2. Five-Stage Architectural Blueprint

### Stage 1: Clarification & Scope
- Daily Active Users (DAU), read-to-write ratio.
- Latency target (e.g., p99 < 100ms) and availability target (e.g., 99.99%).

### Stage 2: High-Level Architecture (HLD)
- Client -> API Gateway / Load Balancer -> Microservices -> Cache -> Primary/Replica DB.
- Asynchronous pipeline: Message Broker (Kafka / RabbitMQ) -> Workers.

### Stage 3: Low-Level Design (LLD) & Data Model
- Data schema, primary keys, indexing strategy, partitioning key.
- REST / gRPC endpoint definitions.

### Stage 4: Resilience & Patterns (.NET Focus)
- Circuit breaker & retry with exponential backoff (`Polly`).
- Distributed caching (`IDistributedCache`, `StackExchange.Redis`).
- Transactional Outbox pattern for reliable event publishing (`MassTransit`).

### Stage 5: Monitoring & Observability
- OpenTelemetry traces, Prometheus metrics, Grafana dashboards, health checks.

