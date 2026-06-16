# Architecture Decision Records (ADRs)

This directory contains all Architecture Decision Records for VisionaryCoder.Tooling.

An Architecture Decision Record (ADR) is a structured document that captures significant architectural decisions, their context, rationale, and consequences.

## Why ADRs?

- **Explicit reasoning** — Document not just *what* was decided, but *why*
- **Searchable history** — Find past decisions and understand evolution
- **Knowledge transfer** — Help new team members understand the architecture
- **Reversibility** — Track which decisions can be revisited if circumstances change
- **Trade-off awareness** — Understand what was sacrificed to gain specific benefits

## Quick Start

1. **Read** existing ADRs in [INDEX.md](INDEX.md) to understand context
2. **Check** [TOC.md](TOC.md) for decisions by category
3. **Create** a new ADR by copying [TEMPLATE.md](TEMPLATE.md)
4. **Follow** the structure in the template and [SKILL.md](../../.agents/skills/adr/SKILL.md)

## Status Overview

| Status | Meaning |
|--------|---------|
| **Proposed** | Under discussion, not yet agreed |
| **Accepted** | Agreed upon, actively used |
| **Superseded** | Replaced by a newer decision |
| **Deprecated** | No longer recommended |
| **Rejected** | Considered but not chosen |

## Naming Convention

```
NNNN-decision-title.md
^^^^                    
Zero-padded sequence number (0001, 0002, etc.)
```

## File Structure

```
docs/adr/
  ├── README.md          ← This file
  ├── INDEX.md           ← Chronological list of all ADRs
  ├── TOC.md             ← Searchable table of contents by category
  ├── TEMPLATE.md        ← Template for new ADRs
  ├── 0001-*.md
  ├── 0002-*.md
  └── ...
```

## How to Create a New ADR

1. Determine the next sequence number by checking the highest in `INDEX.md`
2. Copy `TEMPLATE.md` to `NNNN-decision-title.md`
3. Fill in all sections:
   - **Title** — Concise decision statement
   - **Status** — Start with "Proposed"
   - **Context** — Problem, constraints, and motivation
   - **Decision** — The chosen approach and rationale
   - **Consequences** — Positive and negative impacts
   - **Alternatives** — Options considered and rejected
   - **Related Decisions** — Links to related ADRs
4. Open a PR for review and discussion
5. Update status to "Accepted" when agreed
6. Update [INDEX.md](INDEX.md) and [TOC.md](TOC.md) when committed

## Guidelines

✅ **Do** create ADRs for:
- Major technology choices (frameworks, patterns, platforms)
- Architectural boundaries and decomposition strategies
- API design and contract decisions
- Data storage and persistence choices
- Security and compliance approaches
- Performance trade-offs with lasting impact

❌ **Don't** create ADRs for:
- Minor implementation details
- Bug fixes or patches
- Reversible decisions with low impact
- Routine maintenance tasks

## Reading an ADR

Use the structure to quickly assess:

1. **Status** — Is this still active?
2. **Context** — What problem does this solve?
3. **Decision** — What approach was chosen?
4. **Consequences** — What trade-offs exist?
5. **Alternatives** — What else was considered?

## Referencing ADRs

When linking to an ADR from code or other docs, include the full path and number:

```markdown
See [ADR-0001: Use Volatility-Based Decomposition](docs/adr/0001-use-volatility-based-decomposition.md)
```

In code comments:
```csharp
// Implements ADR-0003 (Incremental Generators Only)
```

## Keeping ADRs Current

When a decision changes:
1. Mark the old ADR as "Superseded by [new ADR link]"
2. Create a new ADR explaining the change
3. Update related documentation
4. Update [INDEX.md](INDEX.md) and [TOC.md](TOC.md)

## Questions?

Refer to the skill documentation at [.agents/skills/adr/SKILL.md](../../.agents/skills/adr/SKILL.md) for detailed guidelines.
