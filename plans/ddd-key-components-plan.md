# Domain-Driven Design (DDD) Key Components & Image Fix Architecture Plan

## 1. Context & Problem Statement
- **Image Rendering Failures**: The existing SVG diagrams (`ddd-strategic-design.svg`, `ddd-tactical-patterns.svg`) failed to render across markdown previewers, VS Code, and web browsers due to unescaped raw ampersands (`&` instead of `&amp;`), rendering them malformed XML documents.
- **Missing Key DDD Components**: The existing documentation in `Architecture/DDD.md` omitted several critical building blocks of Domain-Driven Design:
  - **DDD Repository Pattern** (Aggregates-only rule, collection-oriented abstraction vs generic CRUD).
  - **DDD Factory Pattern** (Complex aggregate genesis, and the critical distinction between Initial Creation and Reconstitution).
  - **Specification Pattern** (Domain boolean predicates, triple utility for in-memory validation, filtering, and SQL query generation).
  - **Domain Discovery via EventStorming** (Sticky note taxonomy, Big Picture, Process Modeling, and Software Design).
  - **Domain Policy / Strategy Pattern** (Pluggable business algorithms).
  - **Result Pattern vs Domain Exceptions** (Railway-oriented programming vs expensive CLR exceptions).

## 2. Implemented Architecture Solutions

### 2.1 SVG Image Vector Corrections & Validation
1. **XML Escaping**: All unescaped ampersands replaced with compliant `&amp;` XML entities across all SVGs.
2. **Tactical Architecture Vector Upgraded** (`images/ddd-tactical-patterns.svg`):
   - Redesigned to explicitly visualize all 8 core tactical building blocks:
     - 1. Aggregate Root (Transactional invariant boundary, private backing fields)
     - 2. Child Entities (Local identity within root)
     - 3. Value Objects (Zero identity, structural equality, immutability)
     - 4. Domain Events (Past-tense immutable business facts)
     - 5. Domain Services (Multi-aggregate pure business logic)
     - 6. Repositories (Interface in Domain, implementation in Infrastructure)
     - 7. Factories (Creation vs Reconstitution)
     - 8. Specifications (Reusable domain query/validation predicates)
3. **EventStorming Collaborative Discovery Vector Added** (`images/event-storming-discovery.svg`):
   - Full color-coded sticky note taxonomy (Domain Event, Command, Aggregate Root, Policy, Read Model, User/Actor, External System) placed on a chronological timeline from left to right.
4. **Validation**: All 4 SVG files verified using Python's `xml.etree.ElementTree` parser (`aggregate-boundary-lifecycle.svg`, `ddd-strategic-design.svg`, `ddd-tactical-patterns.svg`, `event-storming-discovery.svg`).

### 2.2 Comprehensive Key Components Expansion
1. **Repository Pattern (Aggregates-Only)**:
   - Detailed rule: Repositories exist ONLY for Aggregate Roots; never for child entities.
   - Benefits, Trade-offs, Target Tech (`Ardalis.Specification`, EF Core), and C# 12 production code.
2. **Factory Pattern (Initial Creation vs Reconstitution)**:
   - Clear architectural boundary: Reconstitution from database/event stream must never re-raise domain events or re-fire initial validation rules.
   - Benefits, Trade-offs, and C# 12 production code.
3. **Specification Pattern**:
   - Triple utility explained: In-memory validation, in-memory filtering, database querying.
   - Composite specification with `.And()`, `.Or()`, and `.Not()`.
   - Benefits, Trade-offs, Target Tech (`Ardalis.Specification`), and C# 12 production code.
4. **EventStorming Discovery Process**:
   - The 7-color taxonomy and 3 levels of EventStorming (Big Picture, Process Modeling, Software Design).
5. **Domain Policy / Strategy Pattern**:
   - Decoupling variable pricing/discount rules via strategy interfaces.
6. **Result Pattern vs Domain Exceptions**:
   - Avoiding CLR exception stack trace performance penalties; explicit typing with `ErrorOr`.

### 2.3 Strict Dual-Language & Notebook Parity
- **English**:
  - `Architecture/DDD.md`
  - `Architecture/DDD.ipynb`
- **Bangla**:
  - `Architecture/DDD-bn.md`
  - `Architecture/DDD-bn.ipynb`
- Both notebooks validated as syntactically valid JSON conforming to Jupyter Notebook schema (`nbformat: 4`, `nbformat_minor: 2`).
