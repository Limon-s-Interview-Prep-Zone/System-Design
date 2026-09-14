# Workspace Rules

## Plan & Design Documents
- Whenever creating plans, architecture specs, or design blueprints, always create them directly in the workspace directory under `plans/` (e.g., `plans/<feature-or-topic>-plan.md`).
- Ensure the `plans/` directory is created if it does not already exist.
- Link all created plan files in responses using clickable markdown links (e.g., `file:///D:/Interview/System-Design/plans/<filename>.md`).

## Documentation & Notebook Standards (Strict Dual Language & SVG Diagrams)
- **Strict Dual Language Requirement (English & Bangla)**:
  - Whenever creating, expanding, or updating documentation, study guides, architectural notes, or reference topics, **always create both English and Bangla versions simultaneously**.
  - File naming convention for notebooks:
    - English: `<topic-name>.ipynb`
    - Bangla: `<topic-name>-bn.ipynb`
  - If markdown files are created alongside notebooks, maintain exact pairs: `<topic-name>.md` and `<topic-name>-bn.md`.
- **Jupyter Notebook (`.ipynb`) Requirements**:
  - All generated `.ipynb` files must be 100% syntactically valid JSON conforming to Jupyter Notebook schema (`nbformat: 4`, `nbformat_minor: 2`).
- **Diagram Rendering Rule in Notebooks**:
  - Standard Jupyter Notebooks do NOT natively render raw ```` ```mermaid ```` code blocks.
  - **Always** save visual diagrams and architectural sequence flows as standalone `.svg` vector files under a local `images/` directory (e.g., `images/<diagram-name>.svg`).
  - Embed diagrams into both the English and Bangla `.ipynb` (and `.md`) files using standard image markdown: `![Caption](images/<diagram-name>.svg)`.
  - This guarantees that diagrams render offline, on GitHub, in VS Code, and in JupyterLab without requiring third-party extensions.
- **Translation Quality & Parity**:
  - The Bangla translation must maintain strict 1-to-1 parity with the English counterpart across all sections, tables, math formulas, decision trees, and code samples.
  - Preserve core computer science and system design terminology in English (or parenthesized transliteration) for interview precision (e.g., *Full-Duplex*, *Multiplexing*, *Idempotency*, *Head-of-Line Blocking*, *Token Bucket*).

## Architectural Concepts & Pattern Explanation Standard (.NET / C# Focus)
- **Mandatory Structure for Explaining Patterns & Concepts**:
  - Whenever explaining system design patterns, architectural mechanisms, or reliability terms (e.g., *Circuit Breaker*, *Retry with Jitter*, *Bulkhead*, *Rate Limiting*, *Idempotency*, *Outbox Pattern*, *CQRS*, *Event Sourcing*, etc.), **always** include:
    1. **Benefits** (in clear bullet points)
    2. **Drawbacks & Trade-offs** (in clear bullet points)
    3. **Target Technology & NuGet Packages for .NET / C#** (e.g., `Polly`, `Microsoft.Extensions.Http.Resilience`, `Microsoft.AspNetCore.RateLimiting`, `MassTransit`, etc.)
    4. **Minimal Code Example**: A concise, clean, production-grade C# / .NET code snippet demonstrating the configuration or usage of the pattern.
  - This standard applies across all explanations, study guides, and `.ipynb` notebooks (in both English and Bangla versions).
