Implement one roadmap slice with minimal token usage and full quality.

Requirements:

- Use [docs/roadmap.md](docs/roadmap.md) as the master sequence.
- Use [docs/sample-full-implementation.md](docs/sample-full-implementation.md) for sample behavior requirements.
- Use [docs/test-coverage-plan.md](docs/test-coverage-plan.md) for coverage requirements.
- Leverage shared components from:
  - [src/vc.Architecture](src/vc.Architecture)
  - [src/vc.Ifx](src/vc.Ifx)
  - [src/vc.Runtime](src/vc.Runtime)
  - [src/vc.Tooling](src/vc.Tooling)
  - [src/vc.Utility](src/vc.Utility)
- Do not duplicate logic that belongs in shared projects.
- Implement tests for happy path, edge cases, and bad inputs.
- Include failure-path tests where applicable.
- Keep changes scoped to the selected slice.

Return all created/updated file contents needed for a complete, testable slice.