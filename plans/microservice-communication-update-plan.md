# Microservice Communication Overhaul Plan

## Objectives
Upgrade and overhaul `communication/microservice-communication.ipynb` and its paired markdown files to ensure full compliance with the workspace rules in `GEMINI.md`:
1. **Architectural Concepts & Pattern Standards (.NET / C# Focus)**:
   - Include Benefits, Drawbacks & Trade-offs, Target Technology / NuGet Packages, and minimal production-grade C# code for all key patterns:
     - Synchronous vs Asynchronous Communication (gRPC vs Event-Driven via `MassTransit`)
     - API Gateway & Reverse Proxy (`Yarp.ReverseProxy`)
     - Circuit Breaker & Retry with Jitter (`Microsoft.Extensions.Http.Resilience` / `Polly`)
     - Rate Limiting (`Microsoft.AspNetCore.RateLimiting`)
     - Service Discovery (Kubernetes DNS / Consul / Envoy)
2. **SVG Diagram Standard**:
   - Create clear, professional SVG vector diagrams under `communication/images/` and embed them into notebooks using markdown image syntax:
     - `microservice-comm-patterns.svg` (Sync REST/gRPC vs Async Message Broker)
     - `api-gateway-architecture.svg` (Gateway features & reverse proxy pipeline)
     - `circuit-breaker-states.svg` (Closed, Open, Half-Open state transitions)
     - `service-discovery-comparison.svg` (Client-side vs Server-side discovery flows)
     - `proxy-architecture.svg` (Forward Proxy vs Reverse Proxy comparison)
3. **Strict Dual Language Requirement**:
   - Maintain 1-to-1 parity between English and Bangla versions:
     - English Notebook: `communication/microservice-communication.ipynb`
     - English Markdown: `communication/microservice-communication.md`
     - Bangla Notebook: `communication/microservice-communication-bn.ipynb`
     - Bangla Markdown: `communication/microservice-communication-bn.md`
   - Preserve technical interview terms in English/transliteration.
4. **Jupyter Notebook JSON Validity**:
   - Ensure all `.ipynb` files strictly conform to `nbformat: 4` and `nbformat_minor: 2`.

## Execution Phases
- [x] Phase 1: Review existing content & formulate plan.
- [ ] Phase 2: Design and create SVG vector diagrams in `communication/images/`.
- [ ] Phase 3: Rewrite and expand `communication/microservice-communication.ipynb` and `communication/microservice-communication.md`.
- [ ] Phase 4: Create Bangla version `communication/microservice-communication-bn.ipynb` and `communication/microservice-communication-bn.md`.
- [ ] Phase 5: Validate JSON structure and verify rendering compatibility.
