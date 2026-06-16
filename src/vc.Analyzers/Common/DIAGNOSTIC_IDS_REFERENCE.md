# Unified Diagnostic ID Reference

This document describes the unified diagnostic ID system for all vc.Analyzers.

## Format

All diagnostic IDs follow this pattern:

```
VC{Category}{Number:0000}
```

Where:
- **VC** — Prefix for VisionaryCoder.Tooling Analyzers
- **{Category}** — 2-4 letter category code
- **{Number:0000}** — Zero-padded 4-digit number

**Examples:**
- `VCARCH0001` — Architecture, rule 1
- VCAPI0003` — API, rule 3
- `VCPERF0001` — Performance, rule 1

---

## Categories

### Architecture (VCARCH0001-0006)
- `VCARCH0001` — Layering violation
- `VCARCH0002` — Cyclic dependency
- `VCARCH0003` — Namespace boundary violation
- `VCARCH0004` — Project reference violation
- `VCARCH0005` — VBD boundary violation
- `VCARCH0006` — Dependency injection violation

### API (VCAPI0001-0006)
- `VCAPI0001` — Missing ApiController attribute
- `VCAPI0002` — Non-RESTful naming
- `VCAPI0003` — Missing API versioning
- `VCAPI0004` — Response serialization issue
- `VCAPI0005` — Incorrect HTTP method
- `VCAPI0006` — Missing validation

### Data Transfer Object (VCDTO0001-0002)
- `VCDTO0001` — DTO with mutable state
- `VCDTO0002` — Circular reference in DTO

### Mapping (VCMAP0001-0002)
- `VCMAP0001` — Unmapped properties
- `VCMAP0002` — Mapping configuration issue

### Async (VCASYNC0001-0005)
- `VCASYNC0001` — Async void method
- `VCASYNC0002` — Blocking call in async method
- `VCASYNC0003` — Missing Async suffix
- `VCASYNC0004` — Fire-and-forget task
- `VCASYNC0005` — Async overhead in hot path

### Design (VCDESIGN0001-0006)
- `VCDESIGN0001` — Empty catch block
- `VCDESIGN0002` — Broad exception catch
- `VCDESIGN0003` — Swallowed exception
- `VCDESIGN0004` — Immutability violation
- `VCDESIGN0005` — Missing logging
- `VCDESIGN0006` — Missing IDisposable

### VBD Access Vault (VCVBDA0001-0003)
- `VCVBDA0001` — Business logic leakage
- `VCVBDA0002` — Schema mapping missing
- `VCVBDA0003` — Boundary violation

### VBD Engine Vault (VCVBDE0001-0003)
- `VCVBDE0001` — Infrastructure access in engine
- `VCVBDE0002` — Non-determinism detected
- `VCVBDE0003` — State violation

### VBD Manager Vault (VCVBDM0001-0003)
- `VCVBDM0001` — Unstable contract
- `VCVBDM0002` — Feature-specific logic
- `VCVBDM0003` — Missing orchestration

### Distributed (VCDIST0001-0009)
- `VCDIST0001` — Missing Apply method (event sourcing)
- `VCDIST0002` — Unused event (event sourcing)
- `VCDIST0003` — Mutable state in event (event sourcing)
- `VCDIST0004` — Missing async in repository
- `VCDIST0005` — IQueryable leakage in repository
- `VCDIST0006` — Repository contract violation
- `VCDIST0007` — Message contract issue
- `VCDIST0008` — Missing idempotency
- `VCDIST0009` — Missing saga compensation

### Performance (VCPERF0001-0007)
- `VCPERF0001` — LINQ in hot path
- `VCPERF0002` — Allocation in hot path
- `VCPERF0003` — Regex recompilation
- `VCPERF0004` — String concatenation in loop
- `VCPERF0005` — Lock contention
- `VCPERF0006` — Reflection in hot path
- `VCPERF0007` — Boxing in hot path

### Resilience (VCRES0001-0012)
- `VCRES0001` — Missing circuit breaker
- `VCRES0002` — Unused circuit breaker
- `VCRES0003` — Missing retry policy
- `VCRES0004` — Excessive retry policy
- `VCRES0005` — Retry configuration issue
- `VCRES0006` — Missing timeout
- `VCRES0007` — Excessive timeout
- `VCRES0008` — Timeout configuration issue
- `VCRES0009` — Missing resilience policy
- `VCRES0010` — Missing cancellation token
- `VCRES0011` — Missing bulkhead
- `VCRES0012` — Missing backpressure

### Security (VCSEC0001-0009)
- `VCSEC0001` — Hardcoded secret
- `VCSEC0002` — SQL injection vulnerability
- `VCSEC0003` — Missing authorization
- `VCSEC0004` — Unsafe deserialization
- `VCSEC0005` — Weak cryptography
- `VCSEC0006` — XSS vulnerability
- `VCSEC0007` — CSRF vulnerability
- `VCSEC0008` — Missing input validation
- `VCSEC0009` — Sensitive data in logs

### Null Safety (VCNULL0001-0002)
- `VCNULL0001` — Missing null check
- `VCNULL0002` — Nullable warning

### Documentation (VCDOC0001-0002)
- `VCDOC0001` — Missing XML documentation
- `VCDOC0002` — Incomplete XML documentation

### Naming Convention (VCNAMING0001-0002)
- `VCNAMING0001` — Naming convention violation
- `VCNAMING0002` — Naming inconsistency

---

## Using DiagnosticIds Constants

All diagnostic IDs are defined in `DiagnosticIds.cs`. Reference them using constants instead of hardcoded strings:

```csharp
using VisionaryCoder.Tooling.Analyzers.Common;

public sealed class MyAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor MyRule = new(
        id: DiagnosticIds.ArchLayeringViolation,  // Instead of "VCARCH0001"
        title: "Layering violation detected",
        messageFormat: "Component '{0}' violates layering rules.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
    
    // ...
}
```

---

## Adding New Diagnostics

When adding new analyzers:

1. Add a new constant to `DiagnosticIds.cs` following the naming pattern
2. Import `VisionaryCoder.Tooling.Analyzers.Common` in your analyzer
3. Reference the constant in your `DiagnosticDescriptor`
4. Update this reference document

---

## See Also

- `DiagnosticIds.cs` — Complete constant definitions
- `.agents/skills/` — Analyzer implementation guidance
- `ADRs` — Architectural decision records for analyzer conventions
