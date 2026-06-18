---
title: ADR-0001: Use Volatility-Based Decomposition for Architecture
description: Project documentation for ADR-0001: Use Volatility-Based Decomposition for Architecture.
status: active
updated: 2026-06-18
---
# ADR-0001: Use Volatility-Based Decomposition for Architecture

**Status:** Accepted

**Decision Date:** 2026-06-15

**Last Updated:** 2026-06-15

## Context

The VisionaryCoder.Tooling project requires a clear, repeatable approach to decomposing components and defining architectural boundaries. Initial monolithic structures created tight coupling across concerns and made it difficult to:

- Identify which components should change together
- Test components in isolation
- Onboard new team members on architectural intent
- Make decisions about which services or libraries to depend on

We needed a decomposition strategy that maps directly to change drivers and makes explicit which parts of the system should evolve together.

## Decision

We will use **Volatility-Based Decomposition (VBD)** from Juval Lowy's architecture principles as the canonical approach for decomposing components, defining boundaries, and validating architectural decisions.

Each component vault aligns around a single **volatility axis** — a primary reason for change. Component members change together because they are affected by the same change drivers and have the same release cadence.

### Three Vault Types

1. **Manager Vault** — Coordinates policy orchestration and use-case flow (policy volatility axis)
2. **Engine Vault** — Owns algorithmic and computational behavior (algorithm volatility axis)
3. **Access Vault** — Isolates external integration and infrastructure detail (infrastructure volatility axis)

All components apply shared VBD rules (VBD0001–VBD0006) and vault-specific rules (VBD1001–VBD3003).

## Consequences

### Positive
- **Explicit boundaries** — Volatility axis makes component responsibilities clear
- **Reduced coupling** — Components isolate different change drivers
- **Testability** — Clear contracts enable isolation and mocking
- **Team alignment** — Architecture mirrors organizational change drivers
- **Scalability** — Decomposition approach scales from modules to services
- **Tooling integration** — Rules map directly to analyzers, code fixes, and generators

### Negative
- **Initial effort** — Requires refactoring existing code to align with volatility axes
- **Learning curve** — Team must understand VBD principles
- **Enforcement** — Requires disciplined code review and automated checking
- **Over-decomposition risk** — Easy to create too many boundaries if not careful

### Ongoing Investment
- Code reviews must validate VBD rules
- Analyzers and code fixes will enforce rules
- Generators will scaffold boundary contracts

## Alternatives

### Alternative 1: Layered Architecture
- **Description:** Organize by technical layers (presentation, business, data)
- **Advantages:** Familiar pattern, easy to understand initially
- **Disadvantages:** Creates coupling across domains; layers don't map to independent change drivers
- **Why rejected:** Doesn't address the root cause of coupling

### Alternative 2: Microservices Architecture
- **Description:** Decompose by business capabilities into independent deployable units
- **Advantages:** Strong isolation and independent scaling
- **Disadvantages:** Adds operational complexity; solves deployment not decomposition
- **Why rejected:** Premature optimization; we need decomposition first

### Alternative 3: Domain-Driven Design (DDD)
- **Description:** Organize around business subdomains and bounded contexts
- **Advantages:** Aligns with business; helps with naming and identity
- **Disadvantages:** Doesn't capture why boundaries should exist or how they change
- **Why rejected:** Complementary to VBD, not a replacement

## Related Decisions

- **Informs:** [ADR-0002 — Dependency Injection Best Practices](0002-dependency-injection-best-practices.md) (DI enables VBD boundary enforcement)
- **Informs:** [ADR-0003 — Incremental Generators Only](0003-incremental-generators-only.md) (generators scaffold VBD contracts)

## References

- **Juval Lowy**, "Rearchitecting Large Software Systems" (MSDN Magazine)
- **VisionaryCoder.Tooling VBD Rules** — See [.agents/skills/vbd-rules/SKILL.md](.agents/skills/vbd-rules/SKILL.md)
- **VBD Vault Skills** — Manager, Engine, Access vaults in [.agents/skills/](../../.agents/skills/)

## Implementation

VBD is implemented through:

1. **Canonical Rules** (VBD0001–VBD0006) — Shared by all vault types
2. **Vault-Specific Rules** (VBD1001–VBD3003) — Manager/Engine/Access rules
3. **Skill Documentation** — Detailed guidance in `.agents/skills/vbd-*`
4. **Analyzer Rules** — Detect VBD violations
5. **Code Fixes** — Suggest corrections for violations
6. **Generator Rules** — Scaffold boundary contracts

## Questions?

See the VBD skills for detailed implementation guidance:
- [VBD Rules](../../.agents/skills/vbd-rules/SKILL.md)
- [VBD Manager Vault](../../.agents/skills/vbd-manager-vault/SKILL.md)
- [VBD Engine Vault](../../.agents/skills/vbd-engine-vault/SKILL.md)
- [VBD Access Vault](../../.agents/skills/vbd-access-vault/SKILL.md)
