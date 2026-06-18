---
title: Code Generator Expansion Roadmap
description: Project documentation for Code Generator Expansion Roadmap.
status: active
updated: 2026-06-18
---
# Code Generator Expansion Roadmap

This roadmap defines a large-scale plan for expanding code generation across VisionaryCoder.Tooling.

It is intentionally broader than the analyzer and code-fix plans. The target is to move from mostly feature-level generators to a cohesive generator platform with strong contracts, composition, diagnostics, testing, and release discipline.

## Strategic Goals

- Increase delivery speed by generating repetitive architectural code with deterministic output.
- Strengthen architectural consistency with VBD-aligned templates and guardrails.
- Reduce runtime bugs by shifting errors to compile-time diagnostics.
- Improve adoption with clear attributes, samples, and predictable generated APIs.
- Build a durable generator platform that can scale to dozens of generators without entropy.

## Scope and Assumptions

- Existing generator instruction set is the baseline and remains authoritative per generator.
- All generators follow `IIncrementalGenerator` and deterministic emission rules.
- Generated code lives under `Vc.Generated.<Category>` style namespaces unless a specific instruction says otherwise.
- Shared contracts, diagnostics, builders, metadata models, and symbol helpers are centralized.
- Analyzer/code-fix parity is desirable, but generator expansion can proceed independently.

## Roadmap Principles

- One volatility axis per generator concern whenever practical.
- Strongly typed inputs and outputs over stringly APIs.
- Deterministic generation only: stable ordering, stable naming, stable files.
- Fail fast with actionable diagnostics and clear fix guidance.
- Prefer additive generation with non-breaking defaults.
- Keep generated public APIs minimal, explicit, and discoverable.
- Optimize for incremental performance from day one.

## Delivery Waves

## Wave 0: Platform Hardening (Foundation)

Build and stabilize shared infrastructure before major generator growth.

- Shared metadata model unification
  - Standardize symbol and type metadata contracts used by all generators.
  - Add reusable helper abstractions for nullability, constraints, generic arity, and accessibility.
- Emission pipeline standardization
  - Introduce consistent source hint naming rules.
  - Add deterministic member ordering policy and formatting conventions.
  - Standardize partial type merge behavior and generated file headers.
- Unified diagnostics platform
  - Reserve generator diagnostic ID ranges by category.
  - Add shared descriptor builders and common message patterns.
  - Introduce guidance links and suppression strategy docs.
- Performance instrumentation
  - Track generator time and allocations at category level.
  - Add baseline benchmarks for incremental invalidation behavior.
  - Set budget thresholds for generation cost per compilation.
- Testing infrastructure
  - Build shared harness for snapshot, semantic, and incremental tests.
  - Add golden-file diff utilities for generated output verification.
  - Add cross-target test matrix (`netstandard2.0`, modern SDK consumers).

Exit criteria:

- Shared infrastructure package is stable and consumed by all existing generators.
- Determinism and incremental baseline tests are green.
- Diagnostics and file layout conventions are documented and enforced.

## Wave 1: Existing Generator Maturity (Depth Before Breadth)

Bring existing generator set from "works" to "production-grade" completeness.

- Domain category maturation
  - `strong-id-generator`: converters, equality helpers, parsing and formatting options.
  - `state-machine-generator`: transition validation, guard actions, telemetry hooks.
  - `command-handler-generator`: pipeline hooks, validation integration, cancellation propagation.
  - `query-handler-generator`: pagination helpers, projection helpers, caching extension points.
  - `event-generator`: envelope versioning, metadata conventions, tracing propagation.
- Design category maturation
  - `validation-generator`: richer rule composition and shared validator registration.
  - `builder-pattern-generator`: required member workflows and immutable build strategies.
- Dx category maturation
  - `union-generator`: exhaustive pattern helpers, conversion operators, serialization support.
  - `mapping-generator`: profile validation, nested mapping strategies, nullability-aware mapping.
- API category maturation
  - `http-endpoint-generator`: route conventions, result shape helpers, auth policy wiring.
  - `api-client-generator`: resiliency hooks, auth handlers, typed error handling.
- Data category maturation
  - `dto-generator`: versioned DTO generation, flattening and projection options.
  - `repository-generator`: specification pattern support, async streaming, batching helpers.
- Distributed category maturation
  - `openapi-generator`: schema drift diagnostics, client partitioning, auth strategy options.
  - `message-contract-generator`: versioning contracts, compatibility checks, transport adapters.
  - `graphql-type-generator`: resolver contracts, schema diagnostics, naming policy controls.
- Observability category maturation
  - `telemetry-generator`: activity/source naming conventions, tag maps, error event contracts.
- Resilience category maturation
  - `feature-flag-generator`: typed flag access, fallback policies, environment-aware defaults.
  - `configuration-generator`: binding diagnostics, validation hooks, reload-safe wrappers.
- Security category maturation
  - `permissions-generator`: policy naming consistency, role maps, convention diagnostics.

Exit criteria:

- Every existing generator has a "v1 complete" checklist and tests.
- Known TODOs in generator implementations are removed or moved into explicit backlog items.
- Samples demonstrate each generator with realistic composition.

## Wave 2: New High-Impact Generators (Horizontal Expansion)

Add generators that remove high-frequency manual coding and architectural drift.

- Application orchestration generators
  - `workflow-generator`: orchestrated step pipelines with retry and compensation hooks.
  - `use-case-generator`: standardized request/response use-case scaffolds.
  - `policy-pipeline-generator`: composable policy handlers around requests.
- Contract and transport generators
  - `versioned-contract-generator`: compatibility layers and deprecation scaffolding.
  - `integration-event-generator`: publisher/subscriber contracts with metadata rules.
  - `grpc-contract-generator`: service stubs and mapping scaffolds.
- Persistence and data access generators
  - `unit-of-work-generator`: transactional boundaries and context wrappers.
  - `specification-generator`: typed query specifications and combinators.
  - `projection-generator`: read-model projection contracts and mappers.
- API surface generators
  - `problem-details-generator`: standardized RFC7807 responses and extensions.
  - `endpoint-filter-generator`: cross-cutting endpoint filters and registration.
  - `api-version-routing-generator`: version-aware endpoint routing conventions.
- Security and auth generators
  - `policy-authorization-generator`: policies, handlers, and requirement scaffolds.
  - `token-contract-generator`: typed token claims wrappers and validation helpers.
  - `secure-secrets-accessor-generator`: safe config accessor wrappers with diagnostics.
- Reliability and operations generators
  - `retry-policy-generator`: typed retry strategy wrappers and defaults.
  - `timeout-policy-generator`: operation-specific timeout templates.
  - `circuit-breaker-policy-generator`: generated policy registration and usage wrappers.

Exit criteria:

- At least 8-12 new generators delivered with docs and samples.
- No regression in generator compile-time budgets beyond agreed thresholds.
- New generators follow shared diagnostics, metadata, and emission contracts.

## Wave 3: Vertical Packs (Opinionated Bundles)

Deliver packaged generator combinations for common architecture slices.

- CRUD service pack
  - DTO + mapping + endpoint + repository + validation + telemetry bundle.
- Event-driven service pack
  - event + message-contract + telemetry + resilience + permissions bundle.
- API-first service pack
  - OpenAPI client + endpoint + problem-details + auth policy bundle.
- Background processing pack
  - command/query handlers + resilience policies + telemetry + feature flags.
- Secure boundary pack
  - permissions + policy auth + configuration + secure accessor bundle.

Each pack should include:

- preconfigured attributes/templates,
- generated composition root wiring,
- pack-level diagnostics for missing required pieces,
- reference sample projects.

Exit criteria:

- Packs are consumable with minimal setup.
- Pack diagnostics prevent partial or inconsistent adoption.
- End-to-end samples compile and pass tests.

## Wave 4: Enterprise Scale and Governance

Add governance, compliance, and platform-level controls for large repos.

- Governance and policy controls
  - config-driven generator allowlists/denylists.
  - per-project generator policy enforcement.
  - compatibility modes for phased migrations.
- Multi-repo and monorepo support
  - shared config inheritance and override model.
  - deterministic generation across parallel builds.
  - resilient behavior with partial project graphs.
- Compliance and audit readiness
  - generation manifest output for CI auditing.
  - diagnostics for insecure or non-compliant generation options.
  - reproducibility checks in CI.
- Versioning and migration tooling
  - generator package upgrade diagnostics.
  - source-compatible migration hints.
  - optional automated migration scaffolds.

Exit criteria:

- Governance features are documented and validated in CI scenarios.
- Migration paths exist for breaking template changes.
- Enterprise-scale builds remain performant and deterministic.

## Cross-Cutting Backlog (Applies to All Waves)

- Developer experience
  - improved attribute docs and IntelliSense descriptions.
  - generated XML docs for public generated types.
  - clear opt-in/opt-out controls per generator.
- Diagnostics quality
  - message consistency and actionable fix text.
  - category/severity policy standardization.
  - suppression guidance and analyzer synergy.
- Generation safety
  - collision detection for generated symbols.
  - namespace and file hint conflict diagnostics.
  - guardrails for partial class misuse.
- Runtime interoperability
  - source-gen-friendly serialization integrations.
  - AOT/linker-friendly generated code modes.
  - optional trimming-safe emission profiles.
- Documentation and discoverability
  - per-generator quick-start sections.
  - compatibility matrix by SDK/runtime.
  - pattern catalog: when to use each generator.

## Category-Level Expansion Matrix

This matrix extends the existing category model and provides a broad backlog target.

- Domain
  - invariants-generator
  - aggregate-factory-generator
  - domain-service-generator
  - domain-notification-generator
- Design
  - options-pattern-generator
  - dependency-registration-generator
  - composition-root-generator
  - object-mother-generator
- Dx
  - result-type-generator
  - error-catalog-generator
  - paged-response-generator
  - test-data-builder-generator
- API
  - problem-details-generator
  - endpoint-filter-generator
  - idempotency-key-generator
  - endpoint-contract-generator
- Data
  - specification-generator
  - unit-of-work-generator
  - projection-generator
  - query-object-generator
- Distributed
  - outbox-generator
  - inbox-generator
  - saga-state-generator
  - consumer-handler-generator
- Observability
  - metric-contract-generator
  - logger-message-generator
  - health-check-generator
  - trace-context-generator
- Resilience
  - retry-policy-generator
  - timeout-policy-generator
  - bulkhead-policy-generator
  - fallback-policy-generator
- Security
  - policy-auth-generator
  - claims-wrapper-generator
  - secure-endpoint-generator
  - encryption-contract-generator

## Implementation Tracks

Run work in parallel tracks to accelerate delivery while reducing merge friction.

- Track A: shared platform and diagnostics.
- Track B: existing generator maturity and test debt removal.
- Track C: new generators by category backlog.
- Track D: sample applications and documentation.
- Track E: CI governance, performance, and release workflows.

Each track should maintain independent milestones and monthly integration checkpoints.

## Quality Gates

Every generator must pass these gates before "done":

- Contract gate
  - clear attribute contract and defaults.
  - explicit generated API shape documented.
- Diagnostics gate
  - invalid input paths produce precise diagnostics.
  - diagnostic IDs, categories, and severity are stable.
- Determinism gate
  - repeated builds produce byte-stable generated output.
  - ordering and naming deterministic across environments.
- Incremental gate
  - unchanged inputs do not trigger unnecessary regeneration.
  - measured invalidation scope is bounded.
- Test gate
  - unit tests for metadata parsing and emission.
  - snapshot tests for generated source.
  - semantic compile tests for generated code.
- Documentation gate
  - instruction file updated.
  - sample usage added or updated.
  - migration notes included when behavior changes.

## Testing Strategy at Scale

- Unit tests
  - metadata extraction and validation behavior.
  - naming and formatting edge cases.
  - diagnostics for invalid attribute usage.
- Golden snapshot tests
  - one canonical output per major scenario.
  - snapshots for generic, nested, nullable, and inheritance cases.
- Semantic tests
  - generated code compiles in realistic consumer projects.
  - cross-framework test matrix for compatibility.
- Incremental tests
  - verify change isolation and cache correctness.
  - track per-generator invalidation behavior.
- Performance tests
  - allocation and time baselines by generator.
  - regression thresholds in CI.

## Milestones and Suggested Timeline

Suggested pacing assumes a medium-sized team and parallel tracks.

1. Month 1-2: Wave 0 complete.
2. Month 3-5: Wave 1 maturity across existing generators.
3. Month 6-9: Wave 2 new high-impact generators.
4. Month 10-11: Wave 3 vertical packs.
5. Month 12+: Wave 4 governance and enterprise scale.

If team size is smaller, keep wave order but reduce parallel track count.

## Risks and Mitigations

- Risk: generator sprawl and inconsistent contracts.
  - Mitigation: shared contract templates and mandatory quality gates.
- Risk: compile-time performance regressions.
  - Mitigation: incremental benchmarks and CI budget enforcement.
- Risk: diagnostic noise and low trust.
  - Mitigation: severity policy and high-quality actionable messages.
- Risk: breaking generated APIs.
  - Mitigation: compatibility profiles, migration diagnostics, staged deprecations.
- Risk: weak adoption due to complexity.
  - Mitigation: vertical packs, samples, and category quick starts.

## Suggested Execution Order

1. Complete Wave 0 platform hardening.
2. Finish Wave 1 maturity for all existing generators.
3. Deliver top-priority Wave 2 generators in Domain, API, Data, and Resilience.
4. Add first two vertical packs from Wave 3 with full samples.
5. Expand remaining Wave 2 backlog by category.
6. Introduce Wave 4 governance and migration tooling.

This sequence maximizes reliability first, then scales feature breadth without sacrificing platform quality.

## Immediate Next Slice (Practical Start)

Start with a first execution slice that is large but controlled.

- Platform
  - finalize shared metadata contracts and deterministic emission policy.
- Existing generators
  - close top TODO gaps in `state-machine`, `mapping`, and `http-endpoint`.
- New generators
  - implement `problem-details-generator` and `specification-generator`.
- Quality
  - establish baseline perf tests and golden snapshots for the above.
- Documentation
  - add quick-start pages and sample composition for the first slice.

Completing this slice validates the roadmap mechanics before scaling to full backlog volume.
