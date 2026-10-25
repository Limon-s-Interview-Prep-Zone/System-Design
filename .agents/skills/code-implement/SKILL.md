---
name: code-implement
description: >-
  Disciplined code implementation and architectural development workflow. Use this
  skill whenever designing, building, or implementing code solutions. It enforces
  a strict 3-stage lifecycle: create plan in plans/, review plan, and implement code
  strictly inside the code/ directory.
---

# Code Implement Skill

> **Senior-Level Definition:**  
> **Code Implementation is the disciplined translation of validated architectural blueprints into modular, testable, and resilient production code sandboxed strictly within designated target directories.**

This skill enforces a disciplined engineering process: **Plan -> Review -> Execute**. It guarantees that all architecture plans are documented in `plans/` and all generated source code and tests reside strictly within the `code/` directory.

## Core Principles

1. **Plan First**: Formulate an architecture and implementation plan in `plans/<feature-name>-plan.md` before writing code.
2. **Review Plan**: Validate the plan against requirements, edge cases, resilience patterns, and directory boundaries.
3. **Execute in `code/`**: Implement all source files, projects, tests, and configuration files strictly inside `code/`. Never create code files in the repository root or outside `code/`.

---

## 3-Stage Procedure

### Step 1: Create Plan
- Identify the feature or architectural requirement.
- Create a comprehensive markdown plan at `plans/<feature-name>-plan.md`.
- Detail the architecture, components, design patterns (.NET Polly, circuit breaker, etc. if applicable), and a file manifest targeting `code/`.
- Refer to [references/workflow.md](references/workflow.md) for full plan template and section guidelines.

### Step 2: Review Plan
- Verify that the plan fulfills all functional requirements.
- Review error handling, edge cases, resilience, and security.
- Confirm every proposed code file resides within `code/<project-or-module>/`.
- Present the plan or review status before proceeding to execution.

### Step 3: Implement Code inside `code/`
- Create the target folder under `code/` (e.g., `code/<feature-name>/`).
- Write clean, modular, production-grade code.
- If applicable, run syntax verification, tests, or builds.
- Return clickable markdown links using portable relative paths (e.g., `[Plan](plans/<feature>-plan.md)` and `[Code](code/<feature>/Program.cs)`) that work across any OS (Windows, Linux, macOS).

