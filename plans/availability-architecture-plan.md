# High Availability Architecture & Disaster Recovery Plan

## Objectives
Overhaul and upgrade the `Availability/` topic to transform it from a basic introductory note into a comprehensive, principal-engineer-level System Design module. Ensure strict adherence to workspace standards in `GEMINI.md`:
1. Strict dual-language parity (English and Bangla).
2. Standalone `.svg` vector diagrams under `Availability/images/`.
3. Practical .NET 8/9 / C# implementation examples with benefits, trade-offs, and NuGet references.
4. 100% syntactically valid Jupyter Notebooks (`nbformat: 4`, `nbformat_minor: 2`).

---

## Detailed Topic Outline (The 8 Pillars of High Availability)

### 1. Availability Fundamentals, Mathematics & SRE Governance
- Availability Definition: Uptime vs. Downtime calculation.
- The 9s of Availability Table (from 90% to 99.9999999%).
- Component Math: Availability in Series ($A_{total} = \prod A_i$) vs. Parallel ($A_{total} = 1 - \prod (1 - A_i)$).
- Reliability Metrics: MTBF (Mean Time Between Failures), MTTR (Mean Time to Repair), MTTD (Mean Time to Detect).
- SRE Reliability Governance: SLI (Service Level Indicator), SLO (Service Level Objective), SLA (Service Level Agreement), and Error Budget burn rate policies.

### 2. Disaster Recovery (DR) Objectives & Strategies
- RTO (Recovery Time Objective): Maximum tolerable downtime.
- RPO (Recovery Point Objective): Maximum tolerable data loss.
- The 4 DR Tiers (Cost vs. Recovery speed):
  1. Backup & Restore (Hours to Days, lowest cost).
  2. Pilot Light (Minutes to Hours, core DB replicated, compute off).
  3. Warm Standby (Seconds to Minutes, scaled-down replica cluster).
  4. Multi-Region Active-Active (Zero downtime, highest complexity & cost).

### 3. Failover Topologies & Clustering Models
- Active-Passive Failover:
  - Cold Standby vs. Warm Standby vs. Hot Standby.
  - Automated vs. Manual failover trade-offs.
- Active-Active Clustering:
  - Load balancing across live nodes, dual-primary replication, traffic shedding.
- Consensus Protocols & Quorum:
  - Raft / Paxos / etcd consensus mechanics.
  - Split-Brain scenario and prevention via Quorum ($Q = \lfloor N/2 \rfloor + 1$), Fencing Tokens, and STONITH.

### 4. Health Probing, Failure Detection & Telemetry
- Probing Paradigms:
  - Liveness Probes: Detects deadlocks/crashes (restarts container).
  - Readiness Probes: Detects saturation/startup readiness (removes from load balancer).
  - Startup Probes: Protects slow-starting legacy/large applications.
- Heartbeats, Leases, and Brownout/Degraded health detection.
- Deep health checks vs. Shallow ping checks.

### 5. Fault Isolation, Resiliency & Graceful Degradation
- Bulkhead Isolation: Isolating thread pools, memory, and database connection pools.
- Circuit Breaker & Retry with Jitter (Polly resilience pipeline).
- Load Shedding & Request Prioritization: Rejecting lower-priority traffic (429/503) under CPU saturation.
- Graceful Degradation: Serving stale cache or fallback responses when backend fails.

### 6. Data Tier High Availability & Replication
- Single-Leader / Primary-Replica: Synchronous vs. Asynchronous replication trade-offs (RPO = 0 vs. Latency).
- Multi-Leader Replication: Conflicts, Last-Write-Wins (LWW), and CRDTs.
- Leaderless / Quorum-based Replication: Dynamo/Cassandra model ($R + W > N$).
- Automated Promotion & Failover (e.g., Patroni for PostgreSQL, Orchestrator for MySQL).

### 7. Global Routing & Ingress Availability
- Anycast DNS (BGP routing to nearest PoP).
- Geo-DNS & Latency-based Routing (AWS Route 53, Cloudflare).
- GSLB (Global Server Load Balancing).
- Multi-CDN architecture & Origin shield failover.

### 8. Zero-Downtime Deployment & Verification
- Deployment Strategies: Rolling Updates, Blue-Green Deployments, Canary Releases.
- Database Schema Zero-Downtime Migration: Expand-Contract (Parallel Run) pattern.
- Chaos Engineering: Chaos Monkey, fault injection, and automated resilience testing.

---

## Technical Artifacts to Deliver
1. **SVG Vector Diagrams** (under `Availability/images/`):
   - `images/ha-architecture-overview.svg` (The 8 Pillars of High Availability)
   - `images/failover-topologies.svg` (Active-Passive vs. Active-Active vs. Consensus Quorum)
   - `images/dr-strategies-rto-rpo.svg` (Disaster Recovery Tiers: RTO vs. RPO vs. Cost)
   - `images/health-checks-probes.svg` (Liveness, Readiness, Startup Probes & Health Pipeline)
2. **English Content**:
   - `Availability/availability.md`
   - `Availability/availability.ipynb`
3. **Bangla Content**:
   - `Availability/availability-bn.md`
   - `Availability/availability-bn.ipynb`
4. **.NET 8/9 C# Code Implementations**:
   - Production-grade ASP.NET Core Health Checks with Liveness/Readiness endpoints and UI response formatter.
   - Resilient Circuit Breaker and Fallback policy with `Microsoft.Extensions.Http.Resilience` / `Polly`.
