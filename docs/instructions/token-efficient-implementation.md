---
title: Token-Efficient Implementation Guide
description: Project documentation for Token-Efficient Implementation Guide.
status: active
updated: 2026-06-18
---
# Token-Efficient Implementation Guide

Use this guide when implementing roadmap items with minimal token usage.

## Inputs

- [docs/roadmap.md](docs/roadmap.md)
- [docs/analyzers-plan.md](docs/analyzers-plan.md)
- [docs/codefixes-plan.md](docs/codefixes-plan.md)
- [docs/generators-plan.md](docs/generators-plan.md)
- [docs/sample-full-implementation.md](docs/sample-full-implementation.md)
- [docs/test-coverage-plan.md](docs/test-coverage-plan.md)

## Implementation Loop

1. Select one roadmap slice.
2. Identify exact target files.
3. Reuse shared logic from:
   - [src/vc.Architecture](src/vc.Architecture)
   - [src/vc.Ifx](src/vc.Ifx)
   - [src/vc.Runtime](src/vc.Runtime)
   - [src/vc.Tooling](src/vc.Tooling)
   - [src/vc.Utility](src/vc.Utility)
4. Implement minimal coherent changes.
5. Add tests for happy path, edge, bad input, and failure modes.
6. Update sample/doc artifacts if externally visible behavior changed.

## Prompt Template

Use short prompts in this format:

- Goal: one sentence.
- Scope: explicit files/folders.
- Reuse: mention shared project components to leverage.
- Tests: exact coverage expectations.
- Exit checks: compile + targeted tests.

## Quality Rules

- Do not duplicate shared helpers in feature projects.
- Keep generated/analyzer/code-fix outputs deterministic.
- Keep each PR/commit focused on one slice.
- Avoid broad refactors during feature slices.

## Required Test Coverage

For each implemented behavior, include:

- Happy path tests.
- Edge-case tests.
- Bad input tests.
- Failure/recovery tests where relevant.

## Completion Checklist

- Slice behavior implemented.
- Tests added and passing.
- Sample impact evaluated and updated if needed.
- Roadmap progress updated.