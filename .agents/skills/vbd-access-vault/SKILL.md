---
name: vbd-access-vault
description: Build access vault components using Juval Lowy VBD and the shared VBD canonical rules.
---

# VBD Access Vault Skill

This skill builds access vaults. It must apply every rule in [.agents/skills/vbd-rules/SKILL.md](../vbd-rules/SKILL.md).

## Vault Purpose

An access vault owns external integration behavior that changes with infrastructure volatility (datastores, APIs, protocols, auth mechanisms).

## Volatility Axis

Access vault volatility axis: infrastructure and integration detail.

If a concern is business orchestration or algorithmic strategy, it does not belong here.

## Boundary Rules

1. Access vault isolates protocol, persistence, auth, and endpoint specifics.
2. Access vault presents stable contracts to manager/engine vaults.
3. Access vault maps external schemas to internal contracts at the boundary.
4. Access vault does not contain business policy decisions.

## Analyzer Mapping

Use canonical VBD0001-VBD0006 plus access-specific checks:

| Rule ID | Default Severity | Summary |
|---|---|---|
| VBD3001 | Error | Access vault exposes external schema types across boundary |
| VBD3002 | Warning | Access vault contains business policy branching |
| VBD3003 | Warning | Access vault contract is provider-specific without abstraction |

## Code Fix Guidance

1. Introduce DTO/entity mappers at the boundary.
2. Move policy branches to manager vault.
3. Introduce provider-neutral contracts and adapter implementations.

## Generator Guidance

When generating access vault components:

1. Generate boundary mapper interfaces and adapter stubs.
2. Generate provider-specific implementations behind stable abstractions.
3. Emit diagnostics metadata for VBD3001-VBD3003 when inputs couple to vendor types.
