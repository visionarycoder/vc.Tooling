---
title: Dependency Injection Best Practices
name: dependency-injection
description: Apply industry best practices for dependency injection, service registration, lifetimes, composition roots, and testable dependency boundaries.
status: active
updated: 2026-06-18
---

# Dependency Injection Best Practices

Use this skill as the canonical DI guidance for analyzers, code fixes, generators, and human-authored code.

## Intent

Build systems that are modular, testable, maintainable, and explicit about dependency boundaries. DI should reduce coupling and make behavior replaceable by contract.

## Core Principles

1. Depend on abstractions, not concretions.
2. Keep object graph wiring at the composition root.
3. Favor constructor injection for required dependencies.
4. Keep service lifetimes correct and explicit.
5. Fail fast on invalid registrations and lifetime mismatches.
6. Avoid hidden dependencies and service locator usage.

## Composition Root Rules

1. Single Composition Root Rule
- Centralize service registration and container configuration in startup/bootstrap code.
- Domain/application layers must not register framework/container details.

2. Explicit Registration Rule
- Register all runtime dependencies explicitly.
- Avoid broad assembly scanning without filters in critical paths.

3. Validation Rule
- Validate container setup at startup when supported.
- Detect missing registrations and scope violations before serving traffic.

## Injection Rules

1. Constructor Injection First
- Required dependencies must be constructor parameters.
- Optional behavior should use explicit options/policy objects, not nullable service dependencies.

2. Property Injection Restriction
- Avoid property injection except for framework-required scenarios.
- Never rely on property injection for required dependencies.

3. Method Injection Restriction
- Use method injection only for short-lived, operation-specific collaborators.
- Do not use method injection to bypass poor service design.

4. No Service Locator Rule
- Do not inject IServiceProvider (or container primitives) into business services.
- Do not resolve dependencies ad hoc at runtime except in controlled factory/adaptor boundaries.

## Lifetime Rules

1. Singleton Rule
- Singleton services must be stateless or internally thread-safe.
- Singleton services must not depend on scoped/transient state that is not safe to cache.

2. Scoped Rule
- Scoped services may depend on singleton and scoped services, not disposable transient state retained beyond scope.

3. Transient Rule
- Transient services should be lightweight and side-effect free.
- Avoid expensive transients in hot paths without caching/pooling strategy.

4. Captive Dependency Rule
- Never allow singleton to capture scoped dependencies.
- Treat captive dependencies as configuration errors.

## Contract and Boundary Rules

1. Interface Segregation Rule
- Keep service interfaces focused on one capability.
- Split fat interfaces by volatility and usage profile.

2. Stable Abstraction Rule
- Application contracts remain stable while implementations can vary.
- Use adapters at boundaries for third-party SDKs/frameworks.

3. Options and Configuration Rule
- Use typed options for configuration.
- Validate options on startup; reject invalid config early.

## Testing Rules

1. Replaceability Rule
- Every service dependency should be replaceable in tests by fake/mock/stub.

2. Deterministic Construction Rule
- Tests can construct services directly without container for unit tests.

3. Integration Verification Rule
- Add container wiring tests for integration boundaries and module registration.

## Common Anti-Patterns (Disallow)

1. Service Locator Anti-Pattern
- Calling container Resolve/GetService from deep business code.

2. God Service Anti-Pattern
- Services with many unrelated dependencies indicating mixed responsibilities.

3. Ambient Context Abuse
- Global static mutable dependencies used instead of injection.

4. Optional Dependency Smell
- Many nullable dependencies indicating unclear service contracts.

## Analyzer/CodeFix/Generator Alignment Contract

All tooling uses the same rule IDs and severities.

| Rule ID | Default Severity | Summary |
|---|---|---|
| DI0001 | Error | Service locator usage in application/business code |
| DI0002 | Error | Singleton captures scoped dependency (captive dependency) |
| DI0003 | Warning | Required dependency not constructor-injected |
| DI0004 | Warning | Excessive constructor dependencies (possible god service) |
| DI0005 | Warning | Missing registration for concrete usage path |
| DI0006 | Warning | Invalid or missing options validation |
| DI0007 | Warning | Property injection used for required dependency |
| DI0008 | Info | Prefer interface boundary for external integration |

## Required Tooling Behavior

1. Analyzer
- Detect DI0001-DI0008.
- Highlight both declaration and registration call sites where applicable.

2. Code Fix
- Offer non-breaking fixes first:
- Convert service locator calls to constructor injection.
- Add missing registration in composition root.
- Add options validation hooks.
- Mark breaking fixes clearly when constructor signature changes are required.

3. Generator
- Generate registration extensions with explicit lifetimes.
- Generate constructor signatures for required dependencies only.
- Refuse generation when DI0001 or DI0002 violations are present.

## Decision Checklist

Before accepting DI design, answer all:

1. Is there a clear composition root?
2. Are required dependencies constructor-injected?
3. Are lifetimes valid with no captive dependencies?
4. Can all dependencies be replaced in tests?
5. Are configuration/options validated at startup?
6. Are rule IDs and severities aligned across analyzer/code fix/generator?
