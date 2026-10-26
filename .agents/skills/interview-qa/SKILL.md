---
name: interview-qa
description: >-
  This skill guides the Interview Agent in generating high-yield, senior-level interview questions and comprehensive answers on any requested topic, delivered in a bilingual (English + Bangla) format and saved permanently to project documentation (`qa/` directory). It also maintains dual-language Q&A pairing and senior engineering frameworks.
---

# Interview Q&A Skill

> **Senior-Level Definition:**  
> **System Design Interview Q&A is a structured, collaborative problem-solving exercise where an engineer navigates ambiguous requirements, calculates constraints, and defends architectural trade-offs in real time.**  
> It focuses on driving clarity from ambiguity, grounding decisions in back-of-the-envelope math, and proactively mitigating bottlenecks and failure modes.

This skill structures system design interview preparation, questions, solutions, and follow-ups. It enforces senior engineering interview frameworks, back-of-the-envelope estimations, architecture blueprints, resilience trade-offs, and dual-language technical communication.

## Core Directives

1. **Mandatory Content Sections for Every Topic**:
   - **Section 1: The Core Design Problem (5-Stage Framework)**:
     - *Stage 1: Clarifying Questions & Scope* (Functional, Non-Functional, SLAs, Latency budgets).
     - *Stage 2: Capacity Estimation* (Back-of-the-envelope calculations for QPS, bandwidth, 5-year storage, RAM).
     - *Stage 3: High-Level Architecture (HLD)* with standalone vector `.svg` diagram in `images/`.
     - *Stage 4: Deep Dive & 4-Part .NET Pattern Explanations* (Benefits, Drawbacks, Target NuGet, Minimal C# Code).
     - *Stage 5: Resilience, Bottlenecks & Failure Modes*.
   - **Section 2: Top Most Frequently Asked Interview Questions & Comprehensive Answers**:
     - Curate the top 3–5 most asked technical questions for that specific domain.
     - Provide authoritative, senior/staff-level model answers (not superficial bullet points).
   - **Section 3: High-Pressure Follow-Up Questions & Production Counter-Answers**:
     - Provide the exact curveball follow-ups interviewers use to test production scars (e.g., "What happens during a split-brain?", "How do you handle a thundering herd?", "How do you prevent data loss during failover?").
     - Supply the step-by-step mitigation strategy and architectural trade-offs.
   - **Section 4: Senior Architectural Summary Decision Matrix / Cheat Sheet**.

2. **Dual-Language Q&A Persistence (`qa/` Directory)**:
   - Always persist interview notes, guides, and Q&A decks directly inside the `qa/` folder.
   - Maintain exact paired files: `qa/<topic>-Interview-Guide.md` (English) and `qa/<topic>-Interview-Guide-bn.md` (Bangla).
   - Bengali translations must maintain standard English interview terminology (e.g., *Throughput*, *Backpressure*, *Eventual Consistency*, *CAP Theorem*, *Split-Brain*, *High Cardinality*, *Tail-Based Sampling*).

3. **.NET / C# Enterprise Focus**:
   - When discussing implementation specifics or resiliency, include concrete .NET patterns (`Polly`, `Microsoft.Extensions.Http.Resilience`, `MassTransit`, `Yarp.ReverseProxy`, `StackExchange.Redis`, `System.Diagnostics.Metrics`).

---

## Step-by-Step Procedure

### 1. Formulate the Question & Constraints
- Frame real-world interview prompts (e.g., "Design a Distributed Rate Limiter", "Design URL Shortener", "Design Notification Service", "Design WhatsApp/Messenger").
- Detail the interviewer expectations and difficulty level (Mid-Level, Senior, Staff/Principal).

### 2. Follow the 5-Stage Architectural Framework
- Walk through Clarifications, Capacity Math, High-Level SVG Diagram, Deep-Dive .NET Levers, and Resilience.
- Refer to [references/qa-framework.md](references/qa-framework.md) for exact estimation formulas and architectural checkpoints.

### 3. Generate Top Most Frequently Asked Questions & Model Answers
- Provide 3 to 5 of the most common, high-yield interview questions asked on this topic by top tech companies.
- Provide comprehensive, staff-level answers detailing exact architectural trade-offs, protocols, and data models.

### 4. Generate Tough Follow-Up Scenarios & Production Counter-Answers
- Include real-world interviewer curveballs and deep-dive drill scenarios (e.g., "What happens if Redis dies?", "How do you mitigate high-cardinality crashes?", "How do you avoid alert fatigue during a cascading outage?").
- Consult [references/follow-up-questions.md](references/follow-up-questions.md).

### 5. Save to `qa/` & Provide Portable Links
- Always write the resulting guides to `qa/<topic>-Interview-Guide.md` and `qa/<topic>-Interview-Guide-bn.md`.
- Return clickable markdown links using portable relative paths (e.g., `[English Guide](qa/<topic>-Interview-Guide.md)` and `[Bangla Guide](qa/<topic>-Interview-Guide-bn.md)`) that work across any OS (Windows, Linux, macOS).

