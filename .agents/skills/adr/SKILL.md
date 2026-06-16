---
name: adr
description: Document architecture decisions and trade-offs using Architecture Decision Records (ADRs).
---

# Architecture Decision Records (ADRs)

Use this skill to document significant architectural decisions, their context, consequences, and trade-offs.

## Intent

Maintain a searchable, time-stamped record of architectural decisions to:
- Provide context for future developers about why decisions were made
- Track trade-offs and alternatives considered
- Enable informed reversal of decisions when circumstances change
- Facilitate knowledge transfer across the team

## What to Record

Create an ADR for:
- Major technology choices (frameworks, libraries, platforms)
- Architectural patterns and decomposition decisions
- API design and contract decisions
- Data storage and persistence strategies
- Security and authentication approaches
- Performance trade-offs
- Build and deployment automation
- Any decision with significant long-term impact

Do NOT create ADRs for:
- Minor implementation details
- Bug fixes or patches
- Routine maintenance tasks
- Reversible decisions with low impact

## ADR Structure

Each ADR follows [docs/adr/TEMPLATE.md](../../docs/adr/TEMPLATE.md):

1. **Title** — Concise decision title (e.g., "Use Volatile-Based Decomposition for Architecture")
2. **Status** — Proposed, Accepted, Superseded, Deprecated
3. **Context** — Problem statement and constraints
4. **Decision** — The chosen approach and why
5. **Consequences** — Positive and negative impacts
6. **Alternatives** — What was considered and rejected, with reasons
7. **Related Decisions** — Links to related ADRs

## Workflow

1. **Create:** Copy [TEMPLATE.md](../../docs/adr/TEMPLATE.md) to `docs/adr/NNNN-decision-title.md`
   - Use zero-padded sequence number (0001, 0002, etc.)
   - Use kebab-case for the title
2. **Draft:** Fill in all sections in the template
3. **Review:** Present to the team and gather feedback
4. **Accept:** Update status to "Accepted" and commit
5. **Reference:** Link to the ADR from relevant code and documentation
6. **Maintain:** Update status if circumstances change

## Status Meanings

- **Proposed** — Under discussion, feedback welcome
- **Accepted** — Agreed upon and actively followed
- **Superseded** — Replaced by a newer decision (link to successor)
- **Deprecated** — No longer recommended but still in use (link to preferred alternative)
- **Rejected** — Considered but not chosen (explain why if not obvious)

## File Naming

```
docs/adr/NNNN-decision-title.md
         ^^^^                  
         Sequential number, zero-padded to 4 digits
```

Examples:
- `0001-use-volatility-based-decomposition.md`
- `0002-dependency-injection-best-practices.md`
- `0003-incremental-generators-only.md`

## Navigation

- **[Index](../../docs/adr/INDEX.md)** — All ADRs by status and date
- **[Table of Contents](../../docs/adr/TOC.md)** — Searchable overview by category
- **[README](../../docs/adr/README.md)** — Introduction and guidelines

## Decision Checklist

Before writing an ADR, confirm:

1. Is this a significant architectural decision?
2. Does the decision have trade-offs worth documenting?
3. Are there alternatives to consider and reject?
4. Will this decision be referenced by code or other docs?
5. Is there team consensus or known disagreement to record?
