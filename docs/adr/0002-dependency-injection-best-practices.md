# ADR-0002: Apply Dependency Injection Best Practices

**Status:** Accepted

**Decision Date:** 2026-06-15

**Last Updated:** 2026-06-15

## Context

As VisionaryCoder.Tooling grows, the dependency graph becomes increasingly complex. Without clear guidelines:

- Services become tightly coupled to concrete implementations
- Testing becomes difficult because dependencies can't be replaced
- The object graph is unclear, making it hard to understand how components wire together
- Mistakes like service locator usage and captive dependencies accumulate

We needed a consistent, enforceable approach to dependency management that:
- Enables testability through replaceable dependencies
- Makes object construction explicit and declarative
- Prevents common pitfalls like captive dependency violations
- Aligns with [ADR-0001 — Volatility-Based Decomposition](0001-use-volatility-based-decomposition.md)

## Decision

We will apply **industry best practices for Dependency Injection** as the canonical approach for all service design and object composition.

### Key Principles

1. **Constructor Injection First** — Required dependencies must be constructor parameters
2. **Depend on Abstractions** — Use interfaces, not concrete types
3. **Composition Root** — Centralize all wiring in startup/bootstrap code
4. **Explicit Registration** — All runtime dependencies must be registered explicitly
5. **Correct Lifetimes** — Singleton/Scoped/Transient with no captive dependencies
6. **No Service Locator** — Never inject IServiceProvider into business code
7. **Validation on Startup** — Detect missing registrations before serving traffic
8. **Testable by Construction** — Every service can be constructed directly for unit tests

### Violation Rules

Rules DI0001–DI0008 are enforced by analyzers, code fixes, and generators:

| Rule | Severity | Summary |
|------|----------|---------|
| DI0001 | Error | Service locator usage in application code |
| DI0002 | Error | Singleton captures scoped dependency (captive) |
| DI0003 | Warning | Required dependency not constructor-injected |
| DI0004 | Warning | Excessive constructor dependencies (god service) |
| DI0005 | Warning | Missing registration for concrete usage |
| DI0006 | Warning | Invalid or missing options validation |
| DI0007 | Warning | Property injection for required dependency |
| DI0008 | Info | Prefer interface boundary for external integration |

## Consequences

### Positive
- **Testability** — Dependencies can be mocked or stubbed in unit tests
- **Clarity** — Object construction is explicit and traceable
- **Flexibility** — Implementations can be swapped without changing consumers
- **Safety** — Violations are detected at startup or via analyzers
- **Alignment** — DI enables [ADR-0001 — VBD](0001-use-volatility-based-decomposition.md) boundary enforcement

### Negative
- **Initial overhead** — Requires setting up composition root and registrations
- **Verbosity** — Constructor parameters can grow for complex services
- **Learning curve** — Team must understand lifetimes and captive dependencies
- **False positives** — Analyzers may flag legitimate patterns

### Ongoing Investment
- Code reviews must validate DI patterns
- Analyzers enforce DI rules
- Generators scaffold DI registration extensions

## Alternatives

### Alternative 1: Service Locator Pattern
- **Description:** Inject container or service locator; resolve dependencies at runtime
- **Advantages:** Reduces constructor parameter count
- **Disadvantages:** Hides dependencies; makes testing hard; couples to container
- **Why rejected:** Makes dependencies implicit and testing harder

### Alternative 2: Property Injection
- **Description:** Mark properties with [Inject] and populate at runtime
- **Advantages:** Reduces constructor parameter count
- **Disadvantages:** Makes required dependencies optional; hard to validate
- **Why rejected:** Obscures required vs. optional dependencies

### Alternative 3: Manual Composition
- **Description:** Wire objects manually without a container
- **Advantages:** No framework overhead; clear control
- **Disadvantages:** Doesn't scale; error-prone; hard to maintain
- **Why rejected:** Manual composition doesn't scale to large applications

## Related Decisions

- **Informed by:** [ADR-0001 — Volatility-Based Decomposition](0001-use-volatility-based-decomposition.md) (DI boundaries align with volatility axes)
- **Related to:** [ADR-0003 — Incremental Generators Only](0003-incremental-generators-only.md) (generators scaffold registration)

## References

- **Andrew Demeyer**, "Dependency Injection Principles, Practices, and Patterns"
- **Microsoft Docs**, "Dependency Injection in .NET"
- **VisionaryCoder.Tooling DI Skill** — See [.agents/skills/dependency-injection/SKILL.md](.agents/skills/dependency-injection/SKILL.md)

## Implementation

DI best practices are enforced through:

1. **Canonical Rules** (DI0001–DI0008) — Shared by all projects
2. **Skill Documentation** — Detailed guidance in [.agents/skills/dependency-injection/SKILL.md](../../.agents/skills/dependency-injection/SKILL.md)
3. **Analyzer Rules** — Detect DI violations
4. **Code Fixes** — Suggest corrections (convert service locator to constructor injection, etc.)
5. **Generator Rules** — Scaffold registration extensions

## Questions?

See [.agents/skills/dependency-injection/SKILL.md](../../.agents/skills/dependency-injection/SKILL.md) for detailed implementation guidance.
