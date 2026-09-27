# Domain-Driven Design (DDD) Principles & Architecture Plan

## Objectives
Create a comprehensive, production-grade, senior-level architectural guide and interview preparation handbook for **Domain-Driven Design (DDD)** in both English and Bangla. Populate `Architecture/DDD.ipynb` and create all paired deliverables adhering to workspace rules.

## Deliverables
1. **Plan Document**:
   - `file:///D:/Interview/System-Design/plans/ddd-principles-architecture-plan.md`
2. **High-Fidelity SVG Vector Diagrams** (`Architecture/images/`):
   - `ddd-strategic-design.svg`: Problem Space vs. Solution Space, Subdomains, Bounded Contexts, and Context Mapping (ACL, Shared Kernel, OHS/PL).
   - `ddd-tactical-patterns.svg`: Tactical DDD building blocks (Aggregate Root, Entities, Value Objects, Domain Events, Repositories) mapped to Clean/Onion Architecture layers.
   - `aggregate-boundary-lifecycle.svg`: Invariant boundaries, Transaction scope (Rule of 1 Aggregate per Transaction), and Outbox Pattern event dispatching.
3. **Documentation & Notebooks (Dual-Language Parity)**:
   - `Architecture/DDD.md` (English Markdown Reference)
   - `Architecture/DDD-bn.md` (Bangla Markdown Reference)
   - `Architecture/DDD.ipynb` (English Jupyter Notebook conforming strictly to `nbformat: 4`, `nbformat_minor: 2`)
   - `Architecture/DDD-bn.ipynb` (Bangla Jupyter Notebook conforming strictly to `nbformat: 4`, `nbformat_minor: 2`)
4. **Content & Architectural Standard (.NET / C# Focus)**:
   - Strategic Design: Ubiquitous Language, Subdomains (Core, Supporting, Generic), Bounded Contexts, Context Mapping relationships.
   - Tactical Design: Rich vs. Anemic Domain Model, Entities vs. Value Objects, Aggregates & Invariants, Strongly-Typed IDs, Domain Services vs. Application Services, Domain Events, Repositories, Specifications.
   - Architectural Integration: Clean / Hexagonal Architecture, CQRS, Outbox Pattern with EF Core & MassTransit / MediatR.
   - Production C# 12 / .NET 8/9 code examples: Aggregate Root with private collections, Value Objects via C# records, EF Core Fluent API mapping (`OwnsOne`, backing fields), Domain Event publishing.
   - In-depth Senior/Staff/Lead Interview Q&A (12+ comprehensive questions with deep rationale, trade-offs, and practical failure modes).
