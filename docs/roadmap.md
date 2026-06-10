# VisionaryCoder.Tooling Implementation Roadmap

This roadmap describes the incremental steps to build out the solution from scaffolding to full implementation.

## Phase 1: Scaffolding and structure

- [ ] Run `scripts/New-VcSolution.ps1` to create the solution and projects.
- [ ] Commit initial solution structure to GitHub.
- [ ] Populate `README.md` and `docs/README-Architecture.md`.
- [ ] Add `.github/workflows/ci.yml` to build and test the solution.

## Phase 2: Shared infrastructure

- [ ] Implement `Vc.Tooling.Abstractions` with core attributes and marker interfaces.  
- [ ] Implement `Vc.Tooling.Diagnostics` with shared diagnostic descriptors.  
- [ ] Implement `Vc.Generators.Common` with helpers like `IndentedStringBuilder`.  
- [ ] Add TODO stubs in `Vc.Tooling.Metadata` for future metadata models.

## Phase 3: Analyzer stubs

- [ ] Add initial analyzer classes in each `Vc.Analyzers.*` project using the template.
- [ ] For each analyzer, document intent and rules in `docs/README-Analyzers.md`.
- [ ] Add TODO comments describing expected behavior and future rules.

## Phase 4: CodeFix stubs

- [ ] Add initial code fix providers in each `Vc.CodeFixes.*` project using the template.
- [ ] Link each CodeFix to its corresponding diagnostic IDs.
- [ ] Add TODO comments for transformation logic.

## Phase 5: Generator stubs

- [ ] Add initial generators in each `Vc.Generators.*` project using the template.
- [ ] For generators with existing designs (e.g., Security, StateMachine), paste the full implementation where ready.
- [ ] Add TODO comments for incomplete generators, describing metadata and emission plans.

## Phase 6: Samples and usage

- [ ] Implement `Vc.Samples.Basic` to demonstrate a minimal generator + analyzer flow.
- [ ] Implement `Vc.Samples.Advanced` to showcase Security, StateMachine, and other advanced features.
- [ ] Document usage in `docs/README-Generators.md` and `docs/README-CodeFixes.md`.

## Phase 7: Iterative implementation

For each artifact (analyzer, code fix, generator):

- [ ] Replace TODO stubs with full implementations.
- [ ] Add unit tests and integration tests.
- [ ] Update documentation and samples to reflect new capabilities.

## Phase 8: Hardening and release

- [ ] Ensure CI passes with analyzers enabled on the repo itself.
- [ ] Add packaging steps to `release.yml` for NuGet publishing.
- [ ] Tag a release and update `docs/RELEASES.md`.
