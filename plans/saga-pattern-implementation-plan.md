# Saga Pattern E-Commerce Implementation Plan (ASP.NET Core 8 + RabbitMQ + MassTransit + SQLite)

## Objectives
Implement the complete, runnable Real-World E-Commerce Case Study Flow specified in `plans/saga-pattern-plan.md`:
1. **Forward Flow (Happy Path)**:
   - `OrderCreated` ➔ `InventoryReserved` ➔ `PaymentProcessed` ➔ `OrderCompleted`
2. **Failure & Compensation Flow (Backward Recovery)**:
   - `OrderCreated` ➔ `InventoryReserved` ➔ `PaymentFailed` ➔ `ReleaseInventory` (Compensate) ➔ `CancelOrder` (Compensate)

---

## Technical Stack & Architecture
- **Framework**: ASP.NET Core 8.0 Minimal APIs / Web API
- **Distributed Messaging**: MassTransit 8.3 with RabbitMQ transport (`localhost:5672`)
- **State Machine Engine**: MassTransit Automatonymous `MassTransitStateMachine<OrderState>`
- **Database & Persistence**: SQLite via Entity Framework Core (`Microsoft.EntityFrameworkCore.Sqlite`)
- **Reliability Pattern**: MassTransit Transactional Outbox (`UseEntityFrameworkOutbox`)
- **Documentation & Testing**: Swashbuckle Swagger UI & HTTP Request tests

---

## Project Structure (`D:\Interview\System-Design\SagaPatternWebApi`)
```
SagaPatternWebApi/
├── Contracts/                 # Message definitions (Commands & Events)
│   ├── Commands.cs
│   └── Events.cs
├── Data/                      # EF Core DbContext & Entities
│   ├── AppDbContext.cs
│   ├── Order.cs              # Business Entity
│   └── OrderState.cs         # Saga State Machine Entity
├── StateMachines/             # MassTransit Automatonymous State Machine
│   └── OrderStateMachine.cs
├── Consumers/                 # Worker service consumers
│   ├── OrderCommandConsumer.cs
│   ├── InventoryCommandConsumer.cs
│   └── PaymentCommandConsumer.cs
├── Controllers/               # REST API Endpoints for triggering & inspecting Sagas
│   └── OrdersController.cs
├── Program.cs                 # DI container, MassTransit, EF Core, Swagger config
├── appsettings.json
└── SagaPatternWebApi.csproj
```

---

## Execution Phases
- [x] Phase 1: Create implementation plan under `plans/`.
- [ ] Phase 2: Create .NET 8 Web API project and configure NuGet packages.
- [ ] Phase 3: Implement domain contracts, models, and EF Core DbContext.
- [ ] Phase 4: Implement MassTransit Automatonymous `OrderStateMachine`.
- [ ] Phase 5: Implement worker consumers for Order, Inventory, and Payment.
- [ ] Phase 6: Implement REST API endpoints with Swagger UI.
- [ ] Phase 7: Build, run, and verify both Forward and Compensation flows.
