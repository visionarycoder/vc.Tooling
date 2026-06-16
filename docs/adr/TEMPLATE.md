# ADR Template

Use this file as a template for new Architecture Decision Records.

**To create a new ADR:**
1. Copy this file to `NNNN-decision-title.md` (use the next sequence number)
2. Fill in all sections
3. Submit a PR for review
4. Update INDEX.md and TOC.md when merged

---

# ADR-NNNN: Brief Title of the Decision

**Status:** Proposed | Accepted | Superseded | Deprecated | Rejected

**Last Updated:** YYYY-MM-DD

**Decision Date:** YYYY-MM-DD (when status became "Accepted")

## Context

Describe the situation that led to the need for this decision. Include:

- The problem or challenge we faced
- Business, technical, or organizational constraints
- Current state that prompted the decision
- Any driving factors (performance, scalability, maintainability, etc.)
- Who is affected by this decision

Example:
> We needed to decompose monolithic services into maintainable components. Component boundaries had been based on technical layers (presentation, business logic, data), but this led to tight coupling across domains. We required a decomposition strategy that could reduce coupling and make boundaries explicit and testable.

## Decision

State the decision clearly and concisely. Explain:

- What was chosen and why
- The rationale behind the choice
- Key reasoning that led to acceptance
- How this decision addresses the context

Example:
> We will use Volatility-Based Decomposition (VBD) from Juval Lowy's architecture principles. Each component boundary will align around a single volatility axis — a primary reason for change. Components change for the same reason at the same cadence.

## Consequences

Describe both positive and negative impacts:

### Positive Consequences
- What benefits does this decision provide?
- How does it improve the system?
- What problems does it solve?

### Negative Consequences
- What trade-offs are we making?
- What becomes harder?
- What additional effort is required?

### Implementation Effort
- Estimated effort to implement
- Ongoing maintenance costs
- Risk factors

Example:
> **Positive:** Reduces coupling, makes boundaries testable, clarifies responsibility.
> **Negative:** Requires architectural discipline, may create unexpected boundaries, initial learning curve.
> **Effort:** Moderate refactoring to existing code; ongoing code reviews.

## Alternatives

List other options that were considered and explain why they were rejected:

### Alternative 1: [Name]
- **Description:** How it would work
- **Advantages:** Why it might work
- **Disadvantages:** Why it was rejected
- **Trade-offs:** What would be different

### Alternative 2: [Name]
- **Description:**
- **Advantages:**
- **Disadvantages:**
- **Trade-offs:**

Example:
> **Alternative 1: Layered Architecture**
> - Simple and familiar to most developers
> - Rejected: Creates coupling across domains; layers don't map to independent change drivers

> **Alternative 2: Microservices**
> - Provides strong isolation
> - Rejected: Adds operational complexity without solving the core decomposition problem; premature optimization

## Related Decisions

Link to related ADRs that inform or depend on this decision:

- **Related to:** ADR-0001 — [Title] (provides foundation for this decision)
- **Supersedes:** ADR-XXXX — [Title] (this replaces an earlier decision)
- **Superseded by:** ADR-YYYY — [Title] (if this decision is superseded)
- **Informs:** ADR-ZZZZ — [Title] (this decision enables downstream decisions)

## References

Provide sources and external references:

- Books, papers, or articles
- GitHub issues or discussions
- Design documents or RFCs
- Team presentations or recordings

Example:
> - Juval Lowy, "Rearchitecting Large Software Systems" (MSDN Magazine, 2019)
> - N. Ford, M. Richards, and D. Sadalage, "Building Evolutionary Architectures"

## Open Questions

If the decision is still "Proposed," note any outstanding questions or concerns:

- What needs clarification?
- What risks are we still evaluating?
- What feedback are we seeking?

---

## Notes for Authors

- **Be concise:** Each section should be clear and readable
- **Be honest:** Include real trade-offs and concerns
- **Be specific:** Avoid vague language; be concrete about impacts
- **Be thorough:** Address alternatives clearly; show you've considered options
- **Be timeless:** Write so future readers understand the decision even years later

See [README.md](README.md) and [SKILL.md](../../.agents/skills/adr/SKILL.md) for guidelines.
