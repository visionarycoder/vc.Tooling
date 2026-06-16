# Implementation Slices Checklist

Use this checklist to execute roadmap work in low-token increments.

## Slice Template

Each slice should include:

- Goal (single feature bundle).
- Target files.
- Shared project reuse plan.
- Tests: happy path, edge cases, bad inputs, failure paths.
- Exit checks.

## Priority Slices

1. Shared contract and utility consolidation.
2. One analyzer rule bundle with matching code fix and tests.
3. One generator bundle with deterministic output tests.
4. Sample scenario A (happy path) wiring all required projects.
5. Sample scenario B/C/D (edge, bad input, integration failure).
6. Coverage expansion across all test projects.

## Cross-Project Reuse Check

For each slice, confirm explicit usage of relevant projects:

- [src/vc.Architecture](src/vc.Architecture)
- [src/vc.Ifx](src/vc.Ifx)
- [src/vc.Runtime](src/vc.Runtime)
- [src/vc.Tooling](src/vc.Tooling)
- [src/vc.Utility](src/vc.Utility)

## Test Completion Check

Before marking a slice done:

- Happy path covered.
- Edge cases covered.
- Bad input covered.
- Failure behavior covered where applicable.
- All tests pass.