# Saga Pattern Architecture & Distributed Transactions Plan

## Objectives
Build out the `sagapattern/` module from an empty directory into an enterprise-grade, principal-level System Design reference guide that thoroughly answers **"Why do we need the Saga Pattern?"**, compares **Choreography vs. Orchestration**, details **Compensating Transactions**, and provides production-grade **.NET 8/9 C# MassTransit State Machine** implementations.

---

## Workspace Standards Compliance
1. **Strict Dual-Language Parity**:
   - English: `sagapattern/saga-pattern.md` & `sagapattern/saga-pattern.ipynb`
   - Bangla: `sagapattern/saga-pattern-bn.md` & `sagapattern/saga-pattern-bn.ipynb`
2. **Standalone SVG Vector Diagrams** (under `sagapattern/images/`):
   - `images/saga-problem-2pc-vs-saga.svg` (The Death of 2PC & Distributed Database Consistency)
   - `images/choreography-vs-orchestration.svg` (Choreography Event Mesh vs. Orchestration State Machine)
   - `images/compensating-transaction-flow.svg` (Failure Rollback Sequence & Semantic Undos)
   - `images/masstransit-state-machine-saga.svg` (MassTransit Automatonymous State Machine Architecture)
3. **Architectural Standard (.NET / C# Focus)**:
   - Benefits & Drawbacks in bullet points.
   - Target NuGet packages (`MassTransit`, `MassTransit.RabbitMQ`, `MassTransit.EntityFrameworkCoreIntegration`).
   - Production-grade C# code example with State Machine, Correlation IDs, and Compensation triggers.
4. **Notebook JSON Integrity**:
   - 100% syntactically valid JSON conforming to `nbformat: 4`, `nbformat_minor: 2`.

---

## Detailed Topic Outline
1. **The Core Problem: Why Do We Need the Saga Pattern?**
   - The shift to Microservices and the Database-per-Service pattern.
   - Why Two-Phase Commit (2PC / XA Transactions) fails in cloud-native microservices (blocking row locks, coordinator SPOF, latency accumulation, CAP theorem CP bias, lack of 2PC support in modern NoSQL/Cloud APIs).
   - The Dual-Write problem and distributed state consistency.
2. **Saga Fundamentals & Historical Context**
   - Original 1987 Hector Garcia-Molina & Kenneth Salem research paper.
   - Definition: A sequence of local transactions across bounded contexts.
   - Forward Recovery (Retry until success) vs. Backward Recovery (Compensating Transactions).
   - Why compensating transactions are *semantic undos* rather than physical database rollbacks.
3. **Choreography vs. Orchestration: Deep Architectural Comparison**
   - Choreography (Decentralized, event-driven, pub/sub).
   - Orchestration (Centralized command coordinator / state machine).
   - Trade-off matrix: Coupling, cyclic dependencies, observability, blast radius, testing complexity.
4. **The Lack of Isolation in Sagas (ACD without I) & Concurrency Countermeasures**
   - Concurrency anomalies: Lost Updates, Dirty Reads, Fuzzy Reads.
   - Proven SRE & Architectural Countermeasures:
     - Semantic Lock (Pending / Reserved state).
     - Commutative Updates.
     - Pessimistic View / Step Reordering.
     - Optimistic Concurrency with version checks.
5. **Real-World E-Commerce Case Study Flow**
   - Forward Flow: `OrderCreated` ➔ `InventoryReserved` ➔ `PaymentProcessed` ➔ `OrderCompleted`.
   - Failure & Compensation Flow: `OrderCreated` ➔ `InventoryReserved` ➔ `PaymentFailed` ➔ `ReleaseInventory` ➔ `CancelOrder`.
6. **Architectural Standard (.NET / C# Focus)**:
   - MassTransit Automatonymous State Machine Saga.
   - Correlation IDs (`Guid`) linking async messages across services.
   - Outbox Pattern integration for transactional event publishing.
   - EF Core saga state persistence.
7. **System Design Interview Decision Framework**:
   - Decision matrix on when to use 2PC vs. Outbox vs. Choreographed Saga vs. Orchestrated Saga.
