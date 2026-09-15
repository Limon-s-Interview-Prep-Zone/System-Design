# Observability Architecture & Telemetry Engineering Plan

## Objectives
Build out the `Observability/` module from an empty file into an enterprise-grade, principal-level System Design study guide adhering strictly to workspace rules in `GEMINI.md`:
1. Strict dual-language parity (English and Bangla).
2. Standalone `.svg` vector diagrams under `Observability/images/`.
3. Practical .NET 8/9 / C# implementation examples with benefits, trade-offs, and NuGet references.
4. 100% syntactically valid Jupyter Notebooks (`nbformat: 4`, `nbformat_minor: 2`).

---

## Detailed Topic Outline (The Core Pillars of Observability)

### 1. Observability Fundamentals & The Three (+1) Pillars (M.E.L.T. + P)
- Observability vs. Monitoring (Output inspection vs. Internal state inference).
- **Metrics**: Aggregated quantitative time-series data (Counters, Gauges, Histograms, Summaries).
- **Logs**: Discrete event records with structured JSON attributes and severity levels.
- **Distributed Traces**: End-to-end request journeys across service boundaries (Trace IDs, Span IDs, Parent Spans).
- **Continuous Profiling (The 4th Pillar)**: Flame graphs for live production CPU, memory allocation, and thread contention.

### 2. OpenTelemetry (OTel) Architecture & Standard
- OpenTelemetry specification: Unified API vs. SDK separation.
- OTel Collector Pipeline Architecture:
  - **Receivers**: Push/Pull telemetry ingestion (OTLP, Prometheus, Jaeger, Zipkin).
  - **Processors**: Batching, memory limiter, attribute filtering, PII redaction, tail sampling.
  - **Exporters**: OTLP transmission to storage backends (Grafana Tempo, Prometheus, Loki, Datadog).
- OTLP (OpenTelemetry Protocol): Protobuf serialization over gRPC and HTTP/protobuf.

### 3. Distributed Context Propagation & W3C TraceContext
- How telemetry flows across network boundaries without losing parent-child relationships.
- W3C TraceContext Specification:
  - `traceparent`: `version-traceId-parentId-traceFlags` (e.g., `00-4bf92f3577b34da6a3ce929d0e0e4736-00f067aa0ba902b7-01`).
  - `tracestate`: Vendor-specific opaque routing pairs.
- OpenTelemetry Baggage API: Propagating contextual metadata (e.g., `tenant.id`, `user.tier`) across hops.
- Log Correlation: Injecting `TraceId` and `SpanId` into structured log outputs to jump from a log line to a trace graph instantly.

### 4. SRE Observability Frameworks & Methodologies
- **The 4 Golden Signals (Google SRE)**: Latency, Traffic, Errors, Saturation.
- **The RED Method (Tom Wilkie / Microservices)**: Rate, Errors, Duration.
- **The USE Method (Brendan Gregg / Infrastructure)**: Utilization, Saturation, Errors.
- Comparison matrix of when to apply which framework.

### 5. Sampling Strategies & High-Cardinality Data Management
- The Cardinality Explosion Problem in Time-Series Databases (Prometheus/M3DB).
- Head-Based Sampling: Decisions made at the ingress point (fixed percentage, rate limiting).
- Tail-Based Sampling: Decisions made at the collector after request completion (100% errors, high-latency traces retained).
- Storage tiering and cost optimization (hot SSDs vs. cold object storage S3/GCS).

### 6. Alerting, Anomaly Detection & Incident Response
- Symptom-based alerting vs. Cause-based alerting (alert on customer pain, not high CPU).
- Multi-Window Multi-Burn-Rate Alerting based on SLO Error Budgets.
- Alert fatigue prevention and automated remediation playbooks.

### 7. Security, Privacy & PII Redaction
- Preventing sensitive data leaks in telemetry pipelines (passwords, credit cards, SSNs, JWTs).
- Redaction techniques: Agent-side regex filtering vs. Collector Transform Processor.
- Compliance standards: GDPR, HIPAA, PCI-DSS in logging and distributed tracing.

### 8. Architectural Standard (.NET / C# Focus)
- Native .NET Diagnostic APIs:
  - `System.Diagnostics.ActivitySource` & `Activity` (Tracing).
  - `System.Diagnostics.Metrics.Meter` & `Counter<T>` (Metrics).
- OpenTelemetry .NET SDK Configuration.
- Serilog structured JSON logging with TraceId/SpanId enrichment.
- Minimal production-grade C# code examples.

### 9. System Design Interview Decision Framework for Observability
- Decision matrix and checklist for architecting telemetry in distributed systems interviews.

---

## Technical Artifacts to Deliver
1. **SVG Vector Diagrams** (under `Observability/images/`):
   - `images/observability-pillars-overview.svg` (The 3+1 Pillars of Observability)
   - `images/distributed-tracing-context-propagation.svg` (W3C TraceContext & Span Hierarchy)
   - `images/otel-collector-architecture.svg` (OpenTelemetry Collector Pipeline)
   - `images/sampling-strategies.svg` (Head-Based vs. Tail-Based Sampling)
2. **English Content**:
   - `Observability/observability.md`
   - `Observability/observability.ipynb`
3. **Bangla Content**:
   - `Observability/observability-bn.md`
   - `Observability/observability-bn.ipynb`
