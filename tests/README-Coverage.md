# Test Coverage Execution Checklist

Use this checklist with [docs/test-coverage-plan.md](docs/test-coverage-plan.md).

## Per Slice Requirements

- Happy path tests added.
- Edge-case tests added.
- Bad-input tests added.
- Failure-path tests added where applicable.

## Project Coverage Targets

- [tests/vc.Architecture.Tests](tests/vc.Architecture.Tests)
- [tests/vc.Ifx.Tests](tests/vc.Ifx.Tests)
- [tests/vc.Runtime.Tests](tests/vc.Runtime.Tests)
- [tests/vc.Utility.Tests](tests/vc.Utility.Tests)
- [tests/vc.Analyzers.Tests](tests/vc.Analyzers.Tests)
- [tests/vc.CodeFixes.Tests](tests/vc.CodeFixes.Tests)
- [tests/vc.Generators.Tests](tests/vc.Generators.Tests)

## Completion Gate

No feature slice is complete until matching tests pass.