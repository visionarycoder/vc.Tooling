# VisionaryCoder.Tooling Token-Efficient Implementation Roadmap

This roadmap is optimized to reduce implementation token usage while still delivering complete functionality.

Execution model:

- Work in small, deterministic slices.
- Reuse shared patterns and contracts before writing new code.
- Keep each implementation prompt scoped to one slice and one acceptance checklist.
- Use referenced plan documents instead of repeating long context in each request.
- **Check in all changes after each phase completes before starting the next phase.**
- **Pause between phases to avoid exhausting token budget within one context window.**

## Linked Plans

- [Analyzer roadmap](docs/analyzers-plan.md)
- [Code-fix roadmap](docs/codefixes-plan.md)
- [Generator roadmap](docs/generators-plan.md)
- [Full sample implementation](docs/sample-full-implementation.md)
- [Detailed test coverage plan](docs/test-coverage-plan.md)
- [Token-efficient implementation instruction](docs/instructions/token-efficient-implementation.md)
- [Implementation slices checklist](docs/implementation-slices.md)

## Project Responsibility Map

All implementation slices must leverage these projects intentionally:

- [src/vc.Architecture](src/vc.Architecture): contracts, boundaries, and architecture composition rules.
- [src/vc.Ifx](src/vc.Ifx): integration-facing adapters, external contracts, and extension points.
- [src/vc.Runtime](src/vc.Runtime): runtime execution components and hosting behaviors.
- [src/vc.Tooling](src/vc.Tooling): composition root, orchestration, and packaging surface.
- [src/vc.Utility](src/vc.Utility): shared utilities and reusable helpers.

## Phase 1: Baseline and Contract Freeze

- [x] Validate solution and project structure in [vc.Tooling.slnx](vc.Tooling.slnx).
- [x] Freeze shared coding contracts and naming conventions.
- [x] Finalize reusable utility/helper boundaries in [src/vc.Utility](src/vc.Utility).
- [x] Finalize architecture and boundary contracts in [src/vc.Architecture](src/vc.Architecture).

Exit criteria:

- Contract drift is minimized before feature implementation starts.

**Phase 1 gate:** Commit all changes. Run `dotnet build` to confirm baseline compiles. Start a new context window before Phase 2.

---

## Phase 2: Shared Platform First

- [ ] Implement shared abstractions once, reuse everywhere.
- [ ] Implement diagnostics and metadata contracts in one place.
- [ ] Implement reusable runtime and integration seams in [src/vc.Runtime](src/vc.Runtime) and [src/vc.Ifx](src/vc.Ifx).
- [ ] Ensure [src/vc.Tooling](src/vc.Tooling) composes these seams without duplicate logic.

Exit criteria:

- New slices can reference shared components instead of regenerating context and code.

**Phase 2 gate:** Commit all changes. Verify shared projects compile and have no unresolved references. Start a new context window before Phase 3.

---

## Phase 3: Analyzer and Code-Fix Slices

- [ ] Execute analyzer slices from [docs/analyzers-plan.md](docs/analyzers-plan.md).
- [ ] Execute matching code-fix slices from [docs/codefixes-plan.md](docs/codefixes-plan.md).
- [ ] Ship in rule bundles: analyzer rule + code fix + tests in one completion unit.
- [ ] Keep each prompt limited to one bundle and one category.

Exit criteria:

- Analyzer and code-fix parity achieved for implemented rule bundles.
- All analyzer tests pass.

**Phase 3 gate:** Commit all analyzer, code-fix, and test files. Run `dotnet build src/vc.Analyzers/vc.Analyzers.csproj` and `dotnet test tests/vc.Analyzers.Tests` and `dotnet test tests/vc.CodeFixes.Tests`. Start a new context window before Phase 4.

---

## Phase 4: Generator Slices

- [ ] Execute generator waves from [docs/generators-plan.md](docs/generators-plan.md).
- [ ] Prioritize high-value deterministic generators first.
- [ ] Reuse shared metadata/emission helpers before writing category-specific logic.
- [ ] Validate generated output against deterministic and incremental gates.

Exit criteria:

- Generator slices pass deterministic output and performance baselines.
- All generator tests pass.

**Phase 4 gate:** Commit all generator source and test files. Run `dotnet build src/vc.Generators/vc.Generators.csproj` and `dotnet test tests/vc.Generators.Tests`. Start a new context window before Phase 5.

---

## Phase 5: Full Sample Implementation

- [ ] Build the complete sample from [docs/sample-full-implementation.md](docs/sample-full-implementation.md).
- [ ] Ensure all required projects are exercised:
	- [src/vc.Architecture](src/vc.Architecture)
	- [src/vc.Ifx](src/vc.Ifx)
	- [src/vc.Runtime](src/vc.Runtime)
	- [src/vc.Tooling](src/vc.Tooling)
	- [src/vc.Utility](src/vc.Utility)
- [ ] Validate sample build, runtime path, and generated source integration.

Exit criteria:

- Sample demonstrates end-to-end architecture, runtime, integration, tooling, and utility usage.
- Sample builds and all four scenarios execute successfully.

**Phase 5 gate:** Commit all sample files. Run `dotnet build` from root. Start a new context window before Phase 6.

---

## Phase 6: Detailed Coverage and Quality Gates

- [ ] Implement full coverage matrix from [docs/test-coverage-plan.md](docs/test-coverage-plan.md).
- [ ] Add tests for happy path, edge cases, invalid/bad input, and failure modes.
- [ ] Add category-level coverage in all test projects:
	- [tests/vc.Architecture.Tests](tests/vc.Architecture.Tests)
	- [tests/vc.Ifx.Tests](tests/vc.Ifx.Tests)
	- [tests/vc.Runtime.Tests](tests/vc.Runtime.Tests)
	- [tests/vc.Utility.Tests](tests/vc.Utility.Tests)
	- [tests/vc.Analyzers.Tests](tests/vc.Analyzers.Tests)
	- [tests/vc.CodeFixes.Tests](tests/vc.CodeFixes.Tests)
	- [tests/vc.Generators.Tests](tests/vc.Generators.Tests)

Exit criteria:

- Coverage goals achieved and critical risk scenarios tested.
- All test projects pass.

**Phase 6 gate:** Commit all test files. Run `dotnet test` from repo root to confirm full suite is green. Start a new context window before Phase 7.

---

## Phase 7: Release Hardening

- [ ] Run `dotnet build` from repo root on a clean checkout.
- [ ] Run `dotnet test` from repo root; confirm zero failures.
- [ ] Validate deterministic generation: run `dotnet build` twice and confirm generator outputs are byte-stable.
- [ ] Validate diagnostics fire correctly on sample source.
- [ ] Validate code fixes apply cleanly on sample source.
- [ ] Confirm no magic string diagnostic IDs remain in `src/vc.Analyzers`.
- [ ] Confirm all public APIs in generated output have XML documentation.
- [ ] Confirm `samples/FullImplementation` builds and runs successfully.
- [ ] Confirm all linked plan milestones are checked off:
  - [ ] [docs/analyzers-plan.md](docs/analyzers-plan.md)
  - [ ] [docs/codefixes-plan.md](docs/codefixes-plan.md)
  - [ ] [docs/generators-plan.md](docs/generators-plan.md)
  - [ ] [docs/sample-full-implementation.md](docs/sample-full-implementation.md)
  - [ ] [docs/test-coverage-plan.md](docs/test-coverage-plan.md)
- [ ] Update `README.md` with final feature list and usage examples.
- [ ] Add `CHANGELOG.md` entry for release.
- [ ] Add packaging steps to `release.yml` for NuGet publishing.
- [ ] Tag a release commit and push.

Exit criteria:

- Full build and test suite are green on a clean checkout.
- All linked plan milestones are complete.
- Release artifacts are packaged and tagged.

**Phase 7 gate:** Commit all remaining changes. Create release tag. Done.

## Token-Minimized Execution Rules

Use these rules for every implementation task:

1. Reference plan links; do not restate long requirements in prompts.
2. Implement one slice at a time, then verify.
3. Reuse shared helpers; avoid duplicated local implementations.
4. Keep prompts short: goal, target files, acceptance checks.
5. Update progress with deltas only.
6. **Commit and push all changes at each phase gate before continuing.**
7. **Start a fresh context window at each phase gate to avoid token exhaustion.**

## Recommended Slice Order

1. Shared contracts and helpers.
2. Analyzer + code-fix bundle for one category.
3. Generator slice for one category.
4. Sample increment that consumes that bundle.
5. Corresponding tests across happy path, edge cases, and bad inputs.

Repeat until all linked plan milestones are complete.
