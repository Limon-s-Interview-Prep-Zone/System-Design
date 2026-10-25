---
name: system-design-learning
description: >-
  Comprehensive workflow for researching, authoring, and updating system design concepts,
  architectural notes, and Jupyter notebooks. Use this skill whenever creating or updating
  system design study materials, guides, or notebooks. Enforces strict dual-language
  (English and Bangla) parity, standalone SVG diagrams in images/, and the 4-part .NET pattern standard.
---

# System Design Learning Skill

> **Senior-Level Definition:**  
> **System Design Learning is the deliberate study of architectural patterns, distributed systems principles, and real-world failure modes to build sound engineering judgment.**  
> It prioritizes first principles, operational realities, and conscious trade-offs (latency, consistency, throughput, complexity, cost) over simplistic "best practices."

This skill guides the creation and expansion of system design study guides, architecture deep-dives, and Jupyter notebooks in strict accordance with [GEMINI.md](../../../GEMINI.md).

## Core Directives

1. **Strict Dual-Language Requirement (English & Bangla)**:
   - Always author both language versions simultaneously.
   - Notebooks: `<topic-name>.ipynb` (English) and `<topic-name>-bn.ipynb` (Bangla).
   - Markdown documents: `<topic-name>.md` and `<topic-name>-bn.md`.
   - Ensure 1-to-1 parity in sections, tables, math formulas, and code snippets.
   - Preserve standard computer science and architecture terms in English or parenthesized transliteration (e.g., *Full-Duplex*, *Multiplexing*, *Idempotency*, *Head-of-Line Blocking*, *Token Bucket*).

2. **Standalone SVG Vector Diagrams**:
   - Standard Jupyter notebooks do NOT render raw Mermaid code blocks.
   - Always output visual flows and architecture diagrams as standalone `.svg` vector files under `images/<diagram-name>.svg`.
   - Embed diagrams into notebooks and markdown files using:
     `![Caption](images/<diagram-name>.svg)`

3. **Mandatory 4-Part .NET / C# Architecture Pattern**:
   - Whenever introducing or explaining architectural patterns (e.g., *Circuit Breaker*, *Retry with Jitter*, *Bulkhead*, *Rate Limiting*, *Idempotency*, *Outbox Pattern*, *CQRS*, *Event Sourcing*):
     1. **Benefits** (clear bullet points)
     2. **Drawbacks & Trade-offs** (clear bullet points)
     3. **Target Technology & NuGet Packages for .NET / C#** (e.g., `Polly`, `Microsoft.Extensions.Http.Resilience`, `Microsoft.AspNetCore.RateLimiting`, `MassTransit`)
     4. **Minimal Code Example**: Concise, clean, production-grade C# snippet demonstrating configuration or usage.

4. **Valid Jupyter Notebook Schema**:
   - Ensure all `.ipynb` files strictly follow the JSON schema (`nbformat: 4`, `nbformat_minor: 2`).

---

## Step-by-Step Procedure

### 1. Research & Outline
- Identify the core concepts, architectural trade-offs, and visual workflows needed.

### 2. Generate SVG Diagrams
- Render visual architecture and flow diagrams as standalone `.svg` vector files directly into `images/<diagram-name>.svg`.
- Check [references/svg-guidelines.md](references/svg-guidelines.md) for styling and formatting rules.

### 3. Author English Version (`<topic>.ipynb` / `<topic>.md`)
- Structure the content logically (Overview, Architecture, Deep Dive, 4-part .NET Pattern Implementation, Trade-offs).
- Reference [references/notebook-template.md](references/notebook-template.md) for notebook JSON structure.

### 4. Author Bangla Version (`<topic>-bn.ipynb` / `<topic>-bn.md`)
- Maintain exact structural parity with the English counterpart.
- Keep technical terms intact with natural Bengali contextual explanation.

### 5. Validate & Link
- Verify JSON formatting of `.ipynb` files.
- Return clickable markdown links using portable relative paths (e.g., `[Notebook](Architecture/<topic>.ipynb)`) that work across any OS (Windows, Linux, macOS).

