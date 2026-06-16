---
name: vbd-engine-vault
description: Build engine vault components using Juval Lowy VBD and the shared VBD canonical rules.
---

# VBD Engine Vault Skill

This skill builds engine vaults. It must apply every rule in [.agents/skills/vbd-rules/SKILL.md](../vbd-rules/SKILL.md).

## Vault Purpose

An engine vault owns algorithmic and computational behavior that changes with strategy, optimization, and calculation volatility.

## Volatility Axis

Engine vault volatility axis: algorithm and computational strategy.

If a concern is orchestration policy or infrastructure integration detail, it does not belong here.

## Boundary Rules

1. Engine vault contracts are pure capability contracts (compute, evaluate, transform).
2. Engine vault avoids direct storage or transport dependencies.
3. Engine vault is deterministic for same input unless nondeterminism is explicit and injectable.
4. Engine vault does not encode workflow policy sequencing.

## Analyzer Mapping

Use canonical VBD0001-VBD0006 plus engine-specific checks:

| Rule ID | Default Severity | Summary |
|---|---|---|
| VBD2001 | Error | Engine vault references access transport or persistence APIs |
| VBD2002 | Warning | Engine output shape leaks orchestration policy state |
| VBD2003 | Warning | Hidden nondeterminism without explicit dependency injection |

## Code Fix Guidance

1. Introduce interfaces for random/time/environment dependencies.
2. Move policy-state branching to manager vault.
3. Replace concrete infrastructure references with abstractions.

## Generator Guidance

When generating engine vault components:

1. Generate capability-first interfaces and pure operation contracts.
2. Generate injectable strategy points for nondeterministic behavior.
3. Emit diagnostics metadata for VBD2001-VBD2003 when input models are ambiguous.
