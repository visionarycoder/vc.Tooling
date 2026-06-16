# Analyzer Roadmap

This roadmap captures the next analyzer additions that best fit the current suite and avoid overlap with the existing rules.

## Phase 1: Highest-value gaps

Start with analyzers that cover common defects and apply broadly across the codebase.

- CancellationToken propagation
	- Flag public async APIs that omit a token when one is available.
	- Flag async call chains that drop an incoming token.
	- Flag long-running operations that do not accept or forward cancellation.
- Disposal correctness
	- Flag `IDisposable` and `IAsyncDisposable` implementations that are not disposed or awaited correctly.
	- Flag disposable fields that are never released.
- DI lifetime safety
	- Flag singleton services that capture scoped dependencies.
	- Flag lifetime mismatches that can lead to captured state or thread-safety issues.
- API contract consistency
	- Flag controller actions that do not follow the expected response and error patterns.
	- Flag missing `CancellationToken` parameters on public endpoint entry points.

## Phase 2: Security expansion

Expand the security surface after the current hardcoded secret, SQL injection, and authorization coverage.

- Path traversal and unsafe file access
	- Flag user-controlled paths passed to file APIs without validation.
	- Flag directory traversal risks when composing paths.
- Command injection and process launch safety
	- Flag shell command construction from untrusted input.
	- Flag unsafe `ProcessStartInfo` usage.
- SSRF and network trust boundaries
	- Flag raw URL fetches that accept untrusted input.
	- Flag unsafe proxy or redirect handling.
- Weak crypto and randomness
	- Flag obsolete hashes, weak ciphers, and non-cryptographic random generators.

## Phase 3: Performance refinements

Add performance analyzers that focus on common hot-path regressions not yet covered.

- Closure and lambda allocations
	- Flag capture-heavy lambdas in hot paths.
	- Flag allocations from delegates or deferred execution in loops.
- Repeated materialization
	- Flag repeated `ToList`, `ToArray`, or `Count` calls over the same sequence.
- Expensive string work in hot paths
	- Flag repeated interpolation, concatenation, or formatting inside hot loops.

## Phase 4: Design and boundary hygiene

Strengthen architectural boundaries and public API quality.

- Public surface leakage
	- Flag infrastructure or persistence types leaking across layer boundaries.
	- Flag internal implementation types exposed in public contracts.
- Encapsulation and immutability
	- Flag mutable public state on data-transfer and contract types.
	- Flag mutable collections exposed directly from public properties.
- Exception and async hygiene
	- Consolidate catch-all diagnostics where they are still fragmented.
	- Flag async methods that return `void` outside event handlers.

## Phase 5: VBD-specific follow-ups

Use the existing VBD analyzers as the base and add missing checks only where they stay aligned with the volatility boundaries.

- Manager-level orchestration rules
	- Flag business logic accidentally placed in orchestration or policy classes.
- Engine-level determinism rules
	- Flag hidden I/O, time access, or random usage in engine code.
- Access-layer boundary rules
	- Flag schema mapping, persistence leakage, and infrastructure coupling.

## Additional Suggestions

These are not immediate roadmap items, but they are likely useful once the core gaps are covered.

- `ConfigureAwait` usage policy for library code, if the repository wants to enforce it.
- Analyzer support for `CancellationToken` in streaming and background worker APIs.
- Tests-only analyzers that verify generated code or samples do not regress into anti-patterns.
- A small set of Roslyn code fixes for the most actionable diagnostics, especially cancellation and disposal.

## Implementation Order

Recommended order for delivery:

1. CancellationToken propagation.
2. Disposal correctness.
3. DI lifetime safety.
4. API contract consistency.
5. Security expansion.
6. Performance refinements.
7. Design and boundary hygiene.
8. VBD-specific follow-ups.

This sequence gives the best coverage first, keeps overlap low, and preserves the current analyzer naming and rule composition pattern.
