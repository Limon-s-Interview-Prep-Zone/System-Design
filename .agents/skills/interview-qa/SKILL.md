---
name: interview-qa
description: >-
  System design interview preparation and question-answering workflow. Use this skill
  when conducting mock system design interviews, generating interview Q&As, capacity
  estimations, high-level architectures, and deep-dive technical interview evaluations.
  Maintains dual-language English/Bangla Q&A pairing and senior engineering frameworks.
---

# Interview Q&A Skill

> **Senior-Level Definition:**  
> **System Design Interview Q&A is a structured, collaborative problem-solving exercise where an engineer navigates ambiguous requirements, calculates constraints, and defends architectural trade-offs in real time.**  
> It focuses on driving clarity from ambiguity, grounding decisions in back-of-the-envelope math, and proactively mitigating bottlenecks and failure modes.

This skill structures system design interview preparation, questions, solutions, and follow-ups. It enforces senior engineering interview frameworks, back-of-the-envelope estimations, architecture blueprints, resilience trade-offs, and dual-language technical communication.

## Core Directives

1. **Structured 5-Stage System Design Interview Framework**:
   - **Stage 1: Clarifying Questions & Scope**: Define functional requirements, non-functional requirements (SLAs, availability, latency), and what is out of scope.
   - **Stage 2: Capacity Estimation**: Back-of-the-envelope calculations for Read/Write QPS, bandwidth (ingress/egress), storage over 5 years, and memory caching needs.
   - **Stage 3: High-Level Design (HLD)**: Core components, API endpoints, and flow diagrams (with standalone `.svg` vector files under `images/`).
   - **Stage 4: Deep Dive & Data Modeling**: Database choice (SQL vs NoSQL), schema design, partitioning/sharding strategies, and cache eviction policies.
   - **Stage 5: Reliability, Resilience & Bottlenecks**: Single points of failure (SPOF), rate limiting, circuit breaker, retry with jitter, outbox pattern, and disaster recovery.

2. **Dual-Language Q&A Persistence (`qa/` Directory)**:
   - Always persist interview notes, guides, and Q&A decks directly inside the `qa/` folder.
   - Maintain exact paired files: `qa/<topic>-Interview-Guide.md` (English) and `qa/<topic>-Interview-Guide-bn.md` (Bangla).
   - Bengali translations must maintain standard English interview terminology (e.g., *Throughput*, *Backpressure*, *Eventual Consistency*, *CAP Theorem*, *Split-Brain*).

3. **.NET / C# Enterprise Focus**:
   - When discussing implementation specifics or resiliency, include concrete .NET patterns (`Polly`, `Microsoft.Extensions.Http.Resilience`, `MassTransit`, `Yarp.ReverseProxy`, `StackExchange.Redis`).

---

## Step-by-Step Procedure

### 1. Formulate the Question & Constraints
- Frame real-world interview prompts (e.g., "Design a Distributed Rate Limiter", "Design URL Shortener", "Design Notification Service", "Design WhatsApp/Messenger").
- Detail the interviewer expectations and difficulty level (Mid-Level, Senior, Staff/Principal).

### 2. Follow the 5-Stage Framework
- Refer to [references/qa-framework.md](references/qa-framework.md) for exact estimation formulas and architectural checkpoints.

### 3. Generate Follow-Up Scenarios & Trade-offs
- Include common interviewer follow-ups (e.g., "What happens if Redis dies?", "How do you handle hot partitions?").
- Consult [references/follow-up-questions.md](references/follow-up-questions.md).

### 4. Save to `qa/` & Provide Portable Links
- Always write the resulting guides to `qa/<topic>-Interview-Guide.md` and `qa/<topic>-Interview-Guide-bn.md`.
- Return clickable markdown links using portable relative paths (e.g., `[English Guide](qa/<topic>-Interview-Guide.md)` and `[Bangla Guide](qa/<topic>-Interview-Guide-bn.md)`) that work across any OS (Windows, Linux, macOS).

