# Antigravity Agent Skills & Workflows

This directory contains workspace-specific skills and rules that power the AI pair programmer across system design study, technical interview prep, and code implementation.

---

## Available Skills

| Skill | Focus | Workflow Lifecycle | Output Location |
| :--- | :--- | :--- | :--- |
| **[`code-implement`](skills/code-implement/SKILL.md)** | Production Code & Architecture | **Plan $\rightarrow$ Review $\rightarrow$ Execute** | `plans/<feature>-plan.md` & `code/<feature>/...` |
| **[`system-design-learning`](skills/system-design-learning/SKILL.md)** | Deep Dives & Dual-Language Notebooks | **Research $\rightarrow$ SVG Diagrams $\rightarrow$ Dual Notebooks (EN/BN) $\rightarrow$ .NET Patterns** | `<topic>.ipynb`, `<topic>-bn.ipynb`, `images/*.svg` |
| **[`interview-qa`](skills/interview-qa/SKILL.md)** | System Design Mock Interviews & Drills | **Scoping $\rightarrow$ Estimations $\rightarrow$ HLD $\rightarrow$ LLD $\rightarrow$ Failure Modes** | `qa/<topic>-Interview-Guide.md` (EN & BN) |

---

## How to Run These Skills

Antigravity skills are discovered automatically from `.agents/skills/`. You can trigger them either **implicitly (via intent)** or **explicitly (naming the skill)**:

### 1. Explicit Invocation (Recommended for direct control)
Mention the skill name directly in your prompt:

- **To run `code-implement`**:
  > *"Use the `code-implement` skill to design and implement a distributed rate limiter in C# using Redis token bucket."*

- **To run `system-design-learning`**:
  > *"Use the `system-design-learning` skill to create a complete notebook on Kafka vs RabbitMQ message brokers."*

- **To run `interview-qa`**:
  > *"Use the `interview-qa` skill to conduct a senior system design interview for designing WhatsApp messenger."*

---

### 2. Implicit / Semantic Invocation (Natural language)
The AI agent automatically selects the appropriate skill based on the task description:

- **Triggers `code-implement`**:
  - *"Implement...", "Build a microservice for...", "Write code for..."*
  - **What happens:** The agent creates `plans/<feature>-plan.md`, conducts a plan review, and then writes modular code strictly inside `code/`.

- **Triggers `system-design-learning`**:
  - *"Explain the architecture of...", "Create a study guide for...", "Add a notebook on..."*
  - **What happens:** The agent creates paired English (`.ipynb` / `.md`) and Bangla (`-bn.ipynb` / `-bn.md`) files, renders vector diagrams in `images/*.svg`, and adds the 4-part .NET pattern structure.

- **Triggers `interview-qa`**:
  - *"Mock interview on...", "How would you design...", "What are the capacity estimations for..."*
  - **What happens:** The agent guides you through clarifying questions, back-of-the-envelope math (QPS, storage), HLD diagrams, LLD data models, and resilience follow-up questions.

---

## Directory Standards & Boundaries

```text
System-Design/
├── .agents/
│   ├── skills/                       # Skill packages (instructions & references)
│   │   ├── code-implement/
│   │   ├── system-design-learning/
│   │   └── interview-qa/
│   └── rules/                        # Global workspace rules
├── plans/                            # System design plans & coding blueprints
├── code/                             # Isolated destination for all generated source code
├── qa/                               # System design interview Q&A guides (English & Bangla)
└── images/                           # Standalone SVG vector diagrams
```

---

## Core Rules Enforced

- **Dual-Language Parity**: All learning materials maintain strict 1-to-1 parity in English and Bangla.
- **Standalone SVG Graphics**: Standard notebooks do not render raw Mermaid. Diagrams are saved as vector `.svg` files in `images/`.
- **4-Part .NET Pattern Structure**: Every pattern explanation includes *Benefits*, *Drawbacks & Trade-offs*, *Target .NET NuGet packages*, and a *Minimal Code Example*.
- **Code Sandboxing**: No code is placed in the workspace root; all code lives in `code/`.
- **Portable Relative Links**: All file links use relative markdown paths that work seamlessly across any OS (Windows, Linux, macOS).

