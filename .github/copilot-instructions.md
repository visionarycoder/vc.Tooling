# VisionaryCoder.Tooling Copilot Instructions

This file guides Copilot on conventions, skills, specialized prompts, and documentation available in this workspace.

## Table of Contents

1. [Available Skills](#available-skills)
2. [Specialized Prompts](#specialized-prompts)
3. [Generator Instructions](#generator-instructions)
4. [Architecture Decisions](#architecture-decisions)
5. [Architecture and Design](#architecture-and-design)
6. [Conventions and Standards](#conventions-and-standards)

---

## Available Skills

Skills in `.agents/skills/` contain reusable rules and best practices. Load them when working on related tasks.

### VBD (Volatility-Based Decomposition)

- **[VBD Rules](.agents/skills/vbd-rules/SKILL.md)** — Canonical Juval Lowy rules shared by analyzers, code fixes, generators, and human code. Load this first for any architecture/boundary work.
  - Rule IDs: VBD0001–VBD0006
  - Core principles: decompose by volatility, stable contracts, inbound/outbound coupling, no leakage, deterministic composition, testability

- **[VBD Manager Vault](.agents/skills/vbd-manager-vault/SKILL.md)** — Build manager components (policy, orchestration). Must apply VBD Rules.
  - Volatility axis: policy orchestration
  - Rule IDs: VBD1001–VBD1003

- **[VBD Engine Vault](.agents/skills/vbd-engine-vault/SKILL.md)** — Build engine components (algorithms, computation). Must apply VBD Rules.
  - Volatility axis: algorithm and computational strategy
  - Rule IDs: VBD2001–VBD2003

- **[VBD Access Vault](.agents/skills/vbd-access-vault/SKILL.md)** — Build access components (infrastructure, persistence, APIs). Must apply VBD Rules.
  - Volatility axis: infrastructure and integration
  - Rule IDs: VBD3001–VBD3003

### Design and Quality

- **[Dependency Injection](.agents/skills/dependency-injection/SKILL.md)** — Industry best practices for DI, service registration, lifetimes, composition roots, testable boundaries.
  - Rule IDs: DI0001–DI0008
  - Key rules: constructor injection, no service locator, singleton/scoped/transient lifetimes, captive dependency detection

- **[Class/File Conventions](.agents/skills/class-file-conventions/SKILL.md)** — One top-level class per file; class name matches filename. Nested classes ignored.
  - Rule IDs: CFC0001–CFC0002
  - Applies to: all C# source files

- **[Unified Diagnostic IDs](.agents/skills/unified-diagnostic-ids/SKILL.md)** — Centralized system for semantic diagnostic ID definitions across all analyzers. Eliminates magic strings and ensures consistency.
  - Format: `VC{Category}{Number:0000}` (e.g., `VCARCH0001`, `VCAPI0003`)
  - Rule IDs: VDI0001–VDI0005
  - Key files: `DiagnosticIds.cs`, `DIAGNOSTIC_IDS_REFERENCE.md`

- **[Analyzer Rule Composition](.agents/skills/analyzer-rule-composition/SKILL.md)** — Build analyzers as composition roots and move each rule into a dedicated class under a rules subfolder.
  - Rule IDs: ARC0001–ARC0005
  - Core model: one analyzer can compose one or more rule classes
  - Example: `ExceptionSafetyAnalyzer` + `ExceptionSafetyRules/*Rule.cs`

- **[Token-Efficient Implementation](.agents/skills/token-efficient-implementation/SKILL.md)** — Execute implementation in minimal-token slices with mandatory shared-project reuse, sample alignment, and complete test coverage.
  - Rule IDs: TEI0001–TEI0006
  - Core model: one slice at a time with code + tests + validation

### Architecture Decisions

- **[Architecture Decision Records (ADRs)](.agents/skills/adr/SKILL.md)** — Document significant architectural decisions and their rationale using the ADR framework.
  - Use for: major technology choices, patterns, API designs, trade-offs
  - See: [ADR Index](docs/adr/INDEX.md), [ADR Table of Contents](docs/adr/TOC.md), [ADR Template](docs/adr/TEMPLATE.md)
  - Current ADRs: 3 foundational decisions (VBD, DI, Incremental Generators)

---

## Specialized Prompts

Prompts in `.github/prompts/` provide task-specific guidance. Use them when working on:

1. **[Generator Prompt](.github/prompts/generator-prompt.md)** — Full generator implementation workflow.
   - Use when: creating or expanding a source generator
   - Links to: `docs/instructions/*-generator.md`

2. **[Analyzer Prompt](.github/prompts/analyzer-prompt.md)** — Full analyzer implementation workflow.
   - Use when: creating or expanding a Roslyn analyzer

3. **[CodeFix Prompt](.github/prompts/codefix-prompt.md)** — Full code fix provider workflow.
   - Use when: creating or expanding a code fix provider

4. **[Architecture Validation Prompt](.github/prompts/architecture-validation-prompt.md)** — Validate designs against VBD and DI principles.
   - Use when: reviewing architecture or boundary designs

5. **[Implementation Test Suite Prompt](.github/prompts/implementation-test-suite-prompt.md)** — Generate comprehensive test scenarios.
   - Use when: creating unit or integration tests

6. **[Expand Shared Library Prompt](.github/prompts/expand-shared-library-prompt.md)** — Extend Common utilities and helpers.
   - Use when: adding new utilities to Vc.Generators.Common

7. **[Integrate All Components Prompt](.github/prompts/integrate-all-components.md)** — End-to-end integration workflow.
   - Use when: wiring analyzer + code fix + generator together

8. **[Token-Efficient Implementation Prompt](.github/prompts/token-efficient-implementation-prompt.md)** — Compact execution prompt for one implementation slice with full quality gates.
  - Use when: implementing roadmap slices with minimal prompt overhead

---

## Generator Instructions

Generator specifications are in `docs/instructions/`. Each describes input attributes, output contracts, project location, and diagnostics.

## Analyzer Instructions

Analyzer specifications are in `docs/instructions/`. Each describes diagnostic rules, implementation patterns, and integration requirements.

- **[Unified Diagnostic IDs](docs/instructions/unified-diagnostic-ids.md)** — Centralized semantic diagnostic ID management system for all analyzers. Step-by-step implementation guide with examples.
  - Format: `VC{Category}{Number:0000}` (e.g., `VCARCH0001`)
  - Use when: Creating new analyzers, adding diagnostic rules, or migrating legacy analyzers
  - Related skill: [Unified Diagnostic IDs](.agents/skills/unified-diagnostic-ids/SKILL.md)

- **[Analyzer Rule Composition](docs/instructions/analyzer-rule-composition.md)** — Enforces analyzer composition where each rule is a separate class in a rules subfolder.
  - Use when: Creating analyzers with one or more rules, or refactoring monolithic analyzers
  - Related skill: [Analyzer Rule Composition](.agents/skills/analyzer-rule-composition/SKILL.md)

- **[Token-Efficient Implementation](docs/instructions/token-efficient-implementation.md)** — Slice-based implementation workflow optimized for low token usage.
  - Use when: Implementing roadmap milestones in compact, testable increments
  - Related skill: [Token-Efficient Implementation](.agents/skills/token-efficient-implementation/SKILL.md)

---

### By Category

**Domain (`Vc.Generators.Domain`):**
- [strong-id-generator.md](docs/instructions/strong-id-generator.md) — Strong ID value type (Guid wrapper)
- [state-machine-generator.md](docs/instructions/state-machine-generator.md) — Strongly typed state machines
- [command-handler-generator.md](docs/instructions/command-handler-generator.md) — CQRS command handlers
- [query-handler-generator.md](docs/instructions/query-handler-generator.md) — CQRS query handlers
- [event-generator.md](docs/instructions/event-generator.md) — Domain event envelopes

**Design (`Vc.Generators.Design`):**
- [validation-generator.md](docs/instructions/validation-generator.md) — Validator classes from data annotations
- [builder-pattern-generator.md](docs/instructions/builder-pattern-generator.md) — Fluent builders

**Dx (`Vc.Generators.Dx`):**
- [union-generator.md](docs/instructions/union-generator.md) — Discriminated unions
- [mapping-generator.md](docs/instructions/mapping-generator.md) — Object mappers

**API (`Vc.Generators.Api`):**
- [http-endpoint-generator.md](docs/instructions/http-endpoint-generator.md) — Minimal API endpoints
- [api-client-generator.md](docs/instructions/api-client-generator.md) — HttpClient-based API clients

**Data (`Vc.Generators.Data`):**
- [dto-generator.md](docs/instructions/dto-generator.md) — Data transfer objects
- [repository-generator.md](docs/instructions/repository-generator.md) — Repository pattern implementations

**Distributed (`Vc.Generators.Distributed`):**
- [openapi-generator.md](docs/instructions/openapi-generator.md) — Clients from OpenAPI specs
- [message-contract-generator.md](docs/instructions/message-contract-generator.md) — Message envelopes and serialization
- [graphql-type-generator.md](docs/instructions/graphql-type-generator.md) — GraphQL type definitions

**Observability (`Vc.Generators.Observability`):**
- [telemetry-generator.md](docs/instructions/telemetry-generator.md) — Activity wrapping and logging

**Resilience (`Vc.Generators.Resilience`):**
- [feature-flag-generator.md](docs/instructions/feature-flag-generator.md) — Feature flag accessors
- [configuration-generator.md](docs/instructions/configuration-generator.md) — Typed configuration registration

**Security (`Vc.Generators.Security`):**
- [permissions-generator.md](docs/instructions/permissions-generator.md) — RBAC permission constants and policies

---

## Architecture and Design

### Documentation

- **[Roadmap](docs/roadmap.md)** — Phased implementation plan (phases 1–8).
- **[Architecture Notes](docs/architecture.md)** — High-level architecture (currently empty; expand as needed).
- **[Architecture Summary](docs/arch.md)** — Quick reference (currently empty; expand as needed).
- **[Analyzers Plan](docs/analyzers-plan.md)** — Analyzer design notes (currently empty; expand as needed).
- **[CodeFixes Plan](docs/codefixes-plan.md)** — Code-fix delivery roadmap aligned to analyzer rules.
- **[Generators Plan](docs/generators-plan.md)** — Large-scale generator platform roadmap and execution waves.
- **[Full Sample Implementation](docs/sample-full-implementation.md)** — End-to-end sample blueprint spanning architecture/runtime/integration/tooling/utility.
- **[Test Coverage Plan](docs/test-coverage-plan.md)** — Required happy-path, edge-case, bad-input, and failure-path coverage matrix.
- **[Implementation Slices](docs/implementation-slices.md)** — Practical execution checklist for token-efficient implementation slices.

### Key Principles

All work must align with:

1. **Volatility-Based Decomposition (VBD)** — Load [VBD Rules](.agents/skills/vbd-rules/SKILL.md) before creating boundaries.
2. **Dependency Injection** — Load [Dependency Injection](.agents/skills/dependency-injection/SKILL.md) for service design.
3. **Class/File Conventions** — One class per file; name matching; nested classes ignored.
4. **Incremental Generators Only** — All source generators must use `IIncrementalGenerator` pattern.
5. **Shared Utilities** — Use `Vc.Generators.Common` for builders, metadata, diagnostics, and symbol extensions.
6. **Deterministic Output** — Generators must produce stable, reproducible code.

---

## Conventions and Standards

### Namespace Conventions

- **Generator:** `VisionaryCoder.Tooling.Generators`
- **Generated Code:** `Vc.Generated.<Category>` (e.g., `Vc.Generated.Domain`, `Vc.Generated.Dx`)
- **Abstractions/Attributes:** `VisionaryCoder.Tooling.Shared.Attributes`
- **Diagnostics:** `VisionaryCoder.Tooling.Diagnostics`

### Diagnostic ID Patterns

- `VBD####` — VBD rule violations
- `DI####` — Dependency injection violations
- `CFC####` — Class/file convention violations
- `GEN####` — Generator-specific diagnostics

### File Organization

```
src/vc.Generators/
  ├── Abstractions/
  │   └── Attributes/ (all marker attributes)
  ├── Common/
  │   ├── Builders/ (IndentedStringBuilder)
  │   ├── Models/ (TypeMetadata, etc.)
  │   ├── Utilities/ (SourceEmitter, etc.)
  │   └── Diagnostics/ (VcGeneratorDiagnostics)
  ├── Domain/ (StrongId, StateMachine, Command/Query/Event)
  ├── Design/ (Validation, Builder)
  ├── Dx/ (Union, Mapping)
  ├── Api/ (Endpoints, Clients)
  ├── Data/ (DTO, Repository)
  ├── Distributed/ (OpenAPI, MessageContract, GraphQL)
  ├── Observability/ (Telemetry)
  ├── Resilience/ (FeatureFlag, Configuration)
  ├── Security/ (Permissions, JWT, etc.)
  └── ...
```

### Workflow

1. **Start:** Load the appropriate skill(s) from `.agents/skills/`.
2. **Task-Specific:** Use a prompt from `.github/prompts/` for generators, analyzers, or code fixes.
3. **Generator Details:** Read the relevant `docs/instructions/*-generator.md` file.
4. **Implement:** Follow the instruction, apply skills, and use shared utilities.
5. **Validate:** Run tests and ensure diagnostics align with skill rule IDs.

---

## When to Use Each Resource

| Task | Load Skill(s) | Use Prompt | Read Instruction |
|------|---------------|-----------|------------------|
| Design a component boundary | VBD Rules | Architecture Validation | (optional: roadmap) |
| Implement a generator | (none) | Generator | `docs/instructions/*-generator.md` |
| Implement an analyzer | (none) | Analyzer | (analyzer instructions, if exist) |
| Implement a code fix | (none) | CodeFix | (code fix instructions, if exist) |
| Design DI wiring | Dependency Injection | (none) | (none) |
| Review architecture | VBD Rules, Dependency Injection | Architecture Validation | Roadmap |
| Add to Vc.Generators.Common | (none) | Expand Shared Library | (none) |
| Create end-to-end feature | All relevant | Integrate All Components | All relevant instructions |

---

## Summary

This workspace provides:

- **6 reusable skills** for architecture, design, and quality standards
- **7 specialized prompts** for task-specific workflows
- **20 generator instructions** with precise specifications
- **Roadmap and documentation** for implementation phases

Always start by loading the appropriate skill(s), then use prompts and instructions to guide implementation.
