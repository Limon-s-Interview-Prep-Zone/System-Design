# OpenTelemetry Integration Plan for Saga Pattern Web API

## 1. Overview
This plan outlines the architecture, NuGet dependencies, configuration, and verification steps required to add end-to-end **OpenTelemetry (Distributed Tracing & Metrics)** to the **SagaPatternWebApi** (.NET 8 + MassTransit + RabbitMQ + SQLite).

---

## 2. Core Architectural Objectives
- **Distributed Tracing (W3C TraceContext)**: Trace the lifecycle of an order across HTTP API endpoints, RabbitMQ queues/exchanges, MassTransit consumers, EF Core queries, and the Saga State Machine transitions under a single unified `TraceId`.
- **Metrics & Telemetry**: Expose runtime, HTTP, EF Core, and MassTransit message throughput / consumer duration metrics via OpenTelemetry.
- **Exporters**: Support standard OTLP (OpenTelemetry Protocol) export to Jaeger, Grafana Tempo, or .NET Aspire Dashboard, with Console exporter for local zero-dependency verification.

---

## 3. Architecture & Trace Propagation Flow

```
[HTTP Client: POST /checkout]
      │ (Creates Activity: HTTP POST /api/orders/checkout)
      │ Generates TraceId: 4bf92f3577b34da6a3ce929d0e0e4736
      ▼
[MassTransit: bus.Publish<SubmitOrderCommand>]
      │ Injects W3C header 'traceparent' into RabbitMQ message envelope
      ▼
[RabbitMQ Exchange: SubmitOrderCommand] ──> [Queue: OrderCommand]
      │
      ▼
[Consumer: OrderCommandConsumer]
      │ Extracts 'traceparent', creates Child Span (Parent = HTTP POST)
      ├──> [EF Core: dbContext.Orders.AddAsync] (Span: db.query sqlite)
      └──> [context.Publish<OrderCreatedEvent>] (Span: send OrderCreatedEvent)
                 │
                 ▼
[Saga: OrderStateMachine]
      │ Creates Child Span (State: Submitted -> AwaitingPayment)
      └──> [context.Publish<ReserveInventoryCommand>]
                 │
                 ▼
[Consumer: InventoryCommandConsumer]
      │ Child Span: Stock Reserved
      └──> [context.Publish<InventoryReservedEvent>]
                 │
                 ▼
[Consumer: PaymentCommandConsumer]
      │ Child Span: Payment Charged
      └──> [context.Publish<PaymentProcessedEvent>]
                 │
                 ▼
[Consumer: OrderCommandConsumer (Complete)]
      │ Child Span: Order Completed in DB
```

---

## 4. Required NuGet Packages (.NET 8)
- `OpenTelemetry.Extensions.Hosting` (v1.11.x)
- `OpenTelemetry.Instrumentation.AspNetCore` (v1.11.x)
- `OpenTelemetry.Instrumentation.Http` (v1.11.x)
- `OpenTelemetry.Instrumentation.EntityFrameworkCore` (v1.11.0-beta.x)
- `OpenTelemetry.Instrumentation.Runtime` (v1.11.x)
- `OpenTelemetry.Exporter.OpenTelemetryProtocol` (v1.11.x)
- `OpenTelemetry.Exporter.Console` (v1.11.x)

---

## 5. Verification & Observability UI
- Run local Jaeger or Aspire Dashboard via Docker:
  ```bash
  docker run -d --name jaeger -e COLLECTOR_ZIPKIN_HOST_PORT=:9411 -p 16686:16686 -p 4317:4317 -p 4318:4318 jaegertracing/all-in-one:latest
  ```
- Send request to `/api/orders/checkout` or `/api/orders/checkout/fail-payment`.
- Inspect the complete waterfall timeline in Jaeger UI at `http://localhost:16686`.
