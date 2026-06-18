---
title: VBD Canonical Rules
name: vbd-rules
description: Canonical Juval Lowy volatility-based decomposition rules shared by analyzers, code fixes, generators, and human-authored components.
status: active
updated: 2026-06-18
---

# VBD Canonical Rules

Use this skill as the single source of truth for all VBD checks and generation.

## Intent

Apply Juval Lowy's volatility-based decomposition (VBD) to model components around change drivers. A component boundary is valid only when all members of the boundary change for the same reason and at the same cadence.

## Core Principles

1. Decompose by volatility, not by technical layer.
2. Keep stable policies separate from volatile implementation details.
3. Minimize cross-boundary coupling and transitive knowledge.
4. Make boundaries explicit, enforceable, and testable.
5. Prefer dependency inversion from volatile areas toward stable contracts.

## Universal Rules (Apply To All Vault Types)

1. Single Volatility Axis Rule
- Each vault owns exactly one primary volatility axis.
- If two unrelated change drivers are found, split the vault.

2. Stable Contract Rule
- Public contracts must be stable relative to the vault's volatility axis.
- Breaking changes require explicit versioning/migration.

3. Inbound/Outbound Rule
- Inbound dependencies may target stable contracts only.
- Outbound dependencies must not create cyclic coupling.

4. No Leakage Rule
- Internal data models, persistence concerns, and protocol details do not cross the vault boundary.

5. Deterministic Composition Rule
- Construction and wiring must be deterministic and side-effect free.
- Runtime behavior can be dynamic, but graph shape must be explicit.

6. Testability Rule
- Every rule must map to testable analyzer diagnostics and generator preconditions.
- Code fixes must be semantics-preserving unless marked breaking.

## Analyzer/CodeFix/Generator Alignment Contract

All tooling tracks the same rule IDs and severities.

| Rule ID | Default Severity | Summary |
|---|---|---|
| VBD0001 | Error | Multiple volatility axes inside one vault |
| VBD0002 | Error | Boundary leakage of internal implementation detail |
| VBD0003 | Warning | Unstable public contract without versioning |
| VBD0004 | Error | Cyclic dependency across vault boundaries |
| VBD0005 | Warning | Non-deterministic vault composition |
| VBD0006 | Warning | Missing explicit boundary contract |

## Required Tooling Behavior

1. Analyzer
- Detect violations of VBD0001-VBD0006.
- Report location at declaration and all usage sites where practical.

2. Code Fix
- Offer non-breaking fix first.
- If only breaking fix exists, mark as breaking in title and preview.
- Never silently remove behavior.

3. Generator
- Refuse generation when error-level rules are violated.
- Generate explicit boundaries and contracts, never inferred hidden coupling.
- Emit traceable comments/metadata linking output to rule IDs.

## Decision Checklist

Before accepting any vault definition, answer all:

1. What is the single volatility axis?
2. Which contracts are stable, and what is their versioning strategy?
3. What can cross the boundary, and what cannot?
4. Which dependencies are allowed directions?
5. Which diagnostics and code fixes are expected if violated?
