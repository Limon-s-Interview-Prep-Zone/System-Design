# Detailed Code Implementation Workflow Reference

## 1. Plan Structure Guidelines (`plans/<feature>-plan.md`)

Each plan document created by this skill must follow this standard structure:

```markdown
# <Feature Name> Implementation Plan

## 1. Overview & Requirements
- Functional goals and use cases
- Non-functional requirements (throughput, latency, reliability)

## 2. Architecture & Design
- Component flow (Mermaid diagram)
- Design patterns (.NET / C# focus: Outbox, Circuit Breaker, Polly Retry, CQRS, etc.)
- Data models & API contracts

## 3. Target File Manifest
All files MUST be inside `code/`:
- `code/<feature>/Domain/...`
- `code/<feature>/Application/...`
- `code/<feature>/Infrastructure/...`
- `code/<feature>/tests/...`

## 4. Implementation Checklist
- [ ] Task 1: Setup project structure in `code/<feature>`
- [ ] Task 2: Implement core models
- [ ] Task 3: Implement services & business logic
- [ ] Task 4: Add tests and verification

## 5. Verification & Testing Strategy
- Commands to build, run unit tests, or verify behavior.
```

---

## 2. Plan Review Checklist

Prior to writing code, verify:
- [ ] **Directory Boundary Check**: Are all target paths within `code/`?
- [ ] **Resilience Check**: Are transient errors handled with retries, timeouts, and circuit breakers?
- [ ] **Clean Code Check**: Are concerns separated (Domain, Service, Controller/Endpoint)?
- [ ] **Link Validation**: Are plans linked with full `file://` URIs?

---

## 3. Directory Layout Standard

```text
System-Design/
├── plans/
│   └── <feature>-plan.md      # Plans and blueprints
├── code/
│   └── <feature-or-project>/  # All source code, configs, and tests
└── .agents/
    ├── skills/
    │   └── code-implement/   # Skill definitions
    └── rules/                # Hierarchical workspace rules
```

