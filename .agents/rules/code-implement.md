# Code Implementation Rules

## Planning & Output Directories
1. **Plan Location**: All implementation blueprints, architecture specs, and execution plans must be saved directly in `plans/` as `plans/<feature-or-topic>-plan.md`.
2. **Review Requirement**: Before implementing code, review the plan against system design best practices, edge cases, and resilience patterns.
3. **Code Location Sandboxing**: All generated code files, modules, tests, and project assets MUST be saved strictly inside the `code/` directory (e.g., `code/<feature>/...`). Never create code files in the repository root or outside `code/`.
4. **Clickable Links**: Link created plans and code files using portable relative paths (e.g., `plans/<feature>-plan.md` or `code/<feature>/...`) so links function seamlessly across any OS (Windows, Linux, macOS).

