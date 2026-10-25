# E-Commerce Saga Orchestration with MassTransit, RabbitMQ & SQLite

A production-grade implementation of the **Saga Pattern (Orchestration)** using **ASP.NET Core 8**, **MassTransit 8.3 (Automatonymous State Machine)**, **RabbitMQ message broker**, **SQLite (EF Core)**, and **OpenTelemetry (Distributed Tracing & Metrics)**.

---

## Observability & OpenTelemetry (Distributed Tracing)

Every message and Saga state machine transition automatically participates in a single unified distributed trace with W3C `TraceId` propagation:

```
[HTTP POST /api/orders/checkout] (TraceId: 74a498c6154999d373f8e24554e4473a)
    │
    ├──► [MassTransit: SubmitOrderCommand send]
    │       │
    │       ▼ (RabbitMQ traceparent envelope)
    ├──► [Consumer: OrderCommand process]
    │       │
    │       ├──► [EF Core SQLite: INSERT INTO Orders]
    │       └──► [MassTransit: OrderCreatedEvent send]
    │               │
    │               ▼
    ├──► [Saga: OrderState process (Initially -> Submitted)]
    │       └──► [MassTransit: ReserveInventoryCommand send]
    │               │
    │               ▼
    ├──► [Consumer: InventoryCommand process]
    │       └──► [MassTransit: InventoryReservedEvent send]
    │               │
    │               ▼
    ├──► [Consumer: PaymentCommand process]
    │       └──► [MassTransit: PaymentProcessedEvent send]
    │               │
    │               ▼
    └──► [Consumer: OrderCommand complete] (Status = Completed)
```

---

## Architecture & Flows

### 1. Forward Flow (Happy Path)
```
[POST /api/orders/checkout]
       │
       ▼ (SubmitOrderCommand)
[OrderCommandConsumer] ──> Save Order (Status = Created)
       │
       ▼ (OrderCreatedEvent)
[OrderStateMachine] ────> Initialize OrderState Saga (State = Submitted)
       │
       ▼ (ReserveInventoryCommand)
[InventoryCommandConsumer] ──> Reserve Stock (Success)
       │
       ▼ (InventoryReservedEvent)
[OrderStateMachine] ────> Transition (State = AwaitingPayment)
       │
       ▼ (ProcessPaymentCommand)
[PaymentCommandConsumer] ───> Charge Customer (Success)
       │
       ▼ (PaymentProcessedEvent)
[OrderStateMachine] ────> Transition (State = Completed)
       │
       ▼ (CompleteOrderCommand)
[OrderCommandConsumer] ──> Update Order (Status = Completed)
```

### 2. Failure & Compensation Flow (Payment Declined)
```
[POST /api/orders/checkout/fail-payment] ($1500 > $1000 limit)
       │
       ▼ (SubmitOrderCommand)
[OrderCommandConsumer] ──> Save Order (Status = Created)
       │
       ▼ (OrderCreatedEvent)
[OrderStateMachine] ────> Transition (State = Submitted)
       │
       ▼ (ReserveInventoryCommand)
[InventoryCommandConsumer] ──> Reserve Stock (Success)
       │
       ▼ (InventoryReservedEvent)
[OrderStateMachine] ────> Transition (State = AwaitingPayment)
       │
       ▼ (ProcessPaymentCommand)
[PaymentCommandConsumer] ───> Fails! ($1500 exceeds $1000 limit)
       │
       ▼ (PaymentFailedEvent)
[OrderStateMachine] ────> Catches Failure! Transition (State = Cancelled)
       ├──► (ReleaseInventoryCommand) ──> [InventoryCommandConsumer] (Release 2 units back to stock)
       └──► (CancelOrderCommand)      ──> [OrderCommandConsumer] (Set Order Status = Cancelled)
```

---

## Project Structure

```
SagaPatternWebApi/
├── Contracts/
│   ├── Commands.cs       # SubmitOrder, ReserveInventory, ProcessPayment, ReleaseInventory, CancelOrder, CompleteOrder
│   └── Events.cs         # OrderCreated, InventoryReserved, PaymentProcessed, PaymentFailed, etc.
├── Consumers/
│   ├── OrderCommandConsumer.cs      # Handles Submit, Complete, and Cancel Order commands
│   ├── InventoryCommandConsumer.cs  # Handles Reserve and Compensating Release Inventory commands
│   └── PaymentCommandConsumer.cs    # Handles Process Payment command
├── Data/
│   ├── Order.cs          # Business domain entity
│   ├── OrderState.cs     # Saga state machine instance entity (CorrelationId, CurrentState, Version)
│   └── AppDbContext.cs   # EF Core DbContext with WAL mode and Outbox entities
├── StateMachines/
│   └── OrderStateMachine.cs  # MassTransit Automatonymous state machine definition
├── Controllers/
│   └── OrdersController.cs   # REST endpoints to trigger & monitor flows
├── Program.cs            # DI, MassTransit, RabbitMQ, and SQLite initialization
└── test_flows.ps1        # Automated PowerShell test runner for all 3 flows
```

---

## Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- RabbitMQ running locally on default port `5672` (User: `guest`, Pass: `guest`)

---

## Running the Application

1. **Start the API:**
   ```powershell
   cd D:\Interview\System-Design\SagaPatternWebApi
   dotnet run
   ```

2. **Open Swagger UI:**
   Navigate to [http://localhost:5150](http://localhost:5150) in your browser.

---

## Testing Scenarios

### Scenario A: Forward Flow (Happy Path)
```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:5150/api/orders/checkout" `
  -ContentType "application/json" `
  -Body '{"customerId":"alice","amount":150.00,"quantity":2}'
```
Query Order status:
```powershell
Invoke-RestMethod -Uri "http://localhost:5150/api/orders/<OrderId>"
Invoke-RestMethod -Uri "http://localhost:5150/api/orders/<OrderId>/saga-state"
```
**Expected Result:**
- Order Status: `Completed`
- Saga CurrentState: `Completed`

---

### Scenario B: Failure & Compensation Flow (Payment Declined)
```powershell
Invoke-RestMethod -Method Post -Uri "http://localhost:5150/api/orders/checkout/fail-payment?customerId=bob&amount=1500"
```
**Expected Result:**
- Amount ($1500) exceeds threshold ($1000)
- Payment fails ➔ Saga executes compensation `ReleaseInventory`
- Order Status: `Cancelled`
- Saga CurrentState: `Cancelled`
- Reason: `Card declined: Amount exceeds per-transaction limit ($1,000.00)`

---

### Scenario C: Automated Verification Script
Run the automated test runner:
```powershell
cd D:\Interview\System-Design\SagaPatternWebApi
powershell -ExecutionPolicy Bypass -File .\test_flows.ps1
```
