# Code Fix Roadmap

This roadmap defines the next code-fix work for the analyzer suite.

The goal is straightforward: for each analyzer rule that reports an actionable diagnostic, provide a matching deterministic code fix that helps the developer apply the recommended change with minimal friction.

## Roadmap Principles

- One analyzer rule should generally map to one code-fix class.
- Code fixes should be deterministic and conservative.
- Code fixes should preserve semantics unless the analyzer explicitly targets a semantic rewrite.
- Code fixes should be categorized and organized to mirror the analyzer layout.
- When a diagnostic is only informational or ambiguous, prefer a light-guidance fix or code action over a risky rewrite.

## Phase 1: Highest-value fixes

Start with diagnostics where the fix is obvious, mechanical, and safe.

- API fixes
  - Add `[ApiController]` to public controllers flagged by the API design analyzer.
  - Rename or rewrite non-RESTful action names to the expected REST pattern.
  - Add missing API version attributes where the controller already follows a versioned contract.
  - Add response metadata such as `ProducesResponseType` for endpoints that already return a clear shape.
  - Add or improve input validation attributes for endpoint parameters.
- Async and cancellation fixes
  - Append `Async` to async method names when the analyzer reports a missing suffix.
  - Add a `CancellationToken` parameter to methods that already call cancellable APIs.
  - Flow existing cancellation tokens through method calls.
  - Replace synchronous blocking calls with async counterparts when the analyzer can identify a safe replacement.
- Null-safety fixes
  - Insert `ArgumentNullException.ThrowIfNull(...)` for required reference parameters.
  - Add null guards where the analyzer can prove a straightforward fix.
- Security fixes
  - Replace raw SQL string concatenation with parameterized command patterns.
  - Add authorization attributes to endpoints already intended to be secured.
  - Replace obvious hardcoded secrets with configuration lookups or placeholders.

## Phase 2: Design and quality fixes

After the safe mechanical fixes are in place, add design-oriented fixes.

- Exception safety
  - Convert `async void` methods to `async Task` when they are not event handlers.
  - Add logging or rethrow paths to swallowed exception handlers.
  - Narrow overly broad catch blocks where the caught symbol is known.
- Dependency injection and configuration
  - Add constructor parameters for missing dependencies where the type already expects them.
  - Generate registration helpers or extension methods for common service patterns.
  - Add configuration binding calls or options registration stubs where the analyzer already identified a missing configuration path.
- Immutability and DTO fixes
  - Convert mutable DTO properties to `init` setters where the type is used as a data contract.
  - Mark eligible fields `readonly`.

## Phase 3: Resilience fixes

Provide code actions for the resilience suite where the transformation is still deterministic.

- Add retry policy wrappers around obvious outbound calls.
- Add timeout wrappers or cancellation flow to long-running operations.
- Add circuit-breaker policy wrappers where the call site already uses a supported policy abstraction.
- Add missing cancellation token parameters to async outbound methods.

## Phase 4: Performance fixes

Fixes in this area should stay narrow and avoid unnecessary rewrites.

- Replace LINQ in hot paths with simple loops when the analyzer can produce a safe equivalent.
- Replace obvious boxing sites with generic or strongly typed alternatives when the target type is known.
- Replace large-struct pass-by-value patterns with `in` or reference-based alternatives where safe.
- Reduce allocation-heavy local patterns when the analyzer can produce a local rewrite.

## Phase 5: Architecture and VBD fixes

These fixes should be offered only when the analyzer can identify a clearly valid target transformation.

- Move type references to the correct layer or suggest the correct dependency direction.
- Add missing mapping helpers in access-layer components.
- Introduce orchestration stubs in manager components when the analyzer detects feature logic in the wrong place.
- Replace infrastructure access in engine code with boundary abstractions when the abstraction is already available.

## Additional Suggestions

These are useful future additions, but they should come after the core fix set is in place.

- Preview-only code actions for diagnostics that have multiple plausible fixes.
- Multi-fix support for related diagnostics in the same file.
- Code fixes that can optionally add tests or test scaffolding for highly mechanical rewrites.
- Refactorings for analyzer-adjacent conventions such as naming, XML docs, or namespace alignment.

## Suggested Delivery Order

1. API fixes.
2. Async and cancellation fixes.
3. Null-safety fixes.
4. Security fixes.
5. Exception safety fixes.
6. DI and configuration fixes.
7. Resilience fixes.
8. Performance fixes.
9. Architecture and VBD fixes.

This order keeps the highest-value and safest transformations first, while leaving more opinionated rewrites for later phases.