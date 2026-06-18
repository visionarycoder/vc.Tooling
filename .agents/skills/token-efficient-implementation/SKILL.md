---
title: Token-Efficient Implementation Skill
description: Project documentation for Token-Efficient Implementation Skill.
status: active
updated: 2026-06-18
---
# Token-Efficient Implementation Skill

## Overview

Use this skill to implement roadmap work with minimal token overhead while preserving full quality.

Primary goals:

- minimize prompt size,
- maximize reuse of existing project assets,
- enforce complete implementation and test coverage.

## Core Rules

### TEI0001: Slice-Based Delivery

Implement one small slice at a time.

- One slice should target one category or one bounded feature.
- Complete code + tests + docs for that slice before moving on.

### TEI0002: Reference-First Prompts

Prompts should reference plan documents instead of restating long requirements.

- Use links to [docs/roadmap.md](docs/roadmap.md), [docs/generators-plan.md](docs/generators-plan.md), [docs/analyzers-plan.md](docs/analyzers-plan.md), [docs/codefixes-plan.md](docs/codefixes-plan.md).
- Keep prompts short: goal, files, acceptance checks.

### TEI0003: Cross-Project Leverage Required

Each implementation run must explicitly consider and leverage:

- [src/vc.Architecture](src/vc.Architecture)
- [src/vc.Ifx](src/vc.Ifx)
- [src/vc.Runtime](src/vc.Runtime)
- [src/vc.Tooling](src/vc.Tooling)
- [src/vc.Utility](src/vc.Utility)

Avoid re-implementing logic that belongs in shared projects.

### TEI0004: Full Sample Alignment

Implementation must align with the sample blueprint in [docs/sample-full-implementation.md](docs/sample-full-implementation.md).

If a new feature affects runtime, integration, or generation behavior, add or update a sample scenario.

### TEI0005: Coverage Completeness

Every implemented slice must include tests for:

- happy path,
- edge cases,
- bad inputs,
- failure paths where applicable.

Use [docs/test-coverage-plan.md](docs/test-coverage-plan.md) as the coverage contract.

### TEI0006: Deterministic Validation

For analyzers/code-fixes/generators:

- validate deterministic behavior,
- validate diagnostics and unsupported contexts,
- validate semantic correctness of output.

## Execution Pattern

1. Pick one slice from [docs/roadmap.md](docs/roadmap.md).
2. Identify reusable components in shared projects.
3. Implement with smallest coherent file set.
4. Add tests from [docs/test-coverage-plan.md](docs/test-coverage-plan.md).
5. Update sample if behavior is externally visible.
6. Verify and close slice.

## Definition of Done

A slice is done only when:

- implementation compiles,
- tests cover required scenarios,
- diagnostics/behavior are validated,
- sample/docs are updated where required.