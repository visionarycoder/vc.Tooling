---
name: vbd-manager-vault
description: Build manager vault components using Juval Lowy VBD and the shared VBD canonical rules.
---

# VBD Manager Vault Skill

This skill builds manager vaults. It must apply every rule in [.agents/skills/vbd-rules/SKILL.md](../vbd-rules/SKILL.md).

## Vault Purpose

A manager vault coordinates business policies and orchestration logic that changes with workflow and policy volatility.

## Volatility Axis

Manager vault volatility axis: policy orchestration and use-case flow.

If a concern is primarily algorithmic runtime strategy or infrastructure access detail, it does not belong here.

## Boundary Rules

1. Manager vault may depend on stable domain contracts.
2. Manager vault may call engine and access vault contracts, but must not embed their internals.
3. Manager vault must not contain transport, serialization, or storage-specific code.
4. Manager vault APIs express intent in business terms, not technical protocol terms.

## Analyzer Mapping

Use canonical VBD0001-VBD0006 plus manager-specific checks:

| Rule ID | Default Severity | Summary |
|---|---|---|
| VBD1001 | Error | Manager vault contains access implementation detail |
| VBD1002 | Warning | Manager API exposes engine tuning knobs |
| VBD1003 | Warning | Manager method mixes multiple policy axes |

## Code Fix Guidance

1. Extract access detail into access vault contract.
2. Extract algorithm tuning into engine vault contract.
3. Split manager methods by policy axis when VBD1003 is triggered.

## Generator Guidance

When generating manager vault components:

1. Generate business-intent interfaces first.
2. Generate orchestration methods that compose contracts, not concrete dependencies.
3. Emit diagnostics metadata for VBD1001-VBD1003 where generation input is ambiguous.
