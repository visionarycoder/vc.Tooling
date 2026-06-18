---
title: XML Documentation Skill
name: xml-documentation
description: Add consistent XML documentation comments for public and protected APIs (enum, class, record, struct, interface, delegate, members, and type parameters).
status: active
updated: 2026-06-18
---

# XML Documentation Skill

Use this skill to add or fix XML comments for types and members so builds pass documentation checks and API intent is explicit.

## Intent

Produce concise, accurate, deterministic XML docs for externally visible code:
- Types (`class`, `record`, `struct`, `interface`, `enum`, `delegate`)
- Members (constructors, methods, properties, fields, events, indexers)
- Generic type parameters and method type parameters
- Parameters, return values, exceptions, and remarks when meaningful

## Core Rules

1. Document all `public` and `protected` symbols.
2. Match comments to actual behavior; do not invent functionality.
3. Keep summaries brief and factual (one or two sentences).
4. Use `<summary>` on every documented symbol.
5. Use `<param>` for each parameter in declaration order.
6. Use `<returns>` for non-`void` methods and delegate return values.
7. Use `<typeparam>` for each generic type parameter.
8. Use `<exception>` only for exceptions that are explicitly thrown/validated.
9. Reuse existing terminology and naming conventions from the surrounding code.
10. Avoid redundant text like "Gets or sets" unless that style is already used nearby.

## Type-Specific Guidance

### Enums
- Add `<summary>` on the enum type.
- Add `<summary>` on every enum member.
- Describe meaning, not implementation details.

### Classes / Records / Structs / Interfaces
- Add `<summary>` on the type.
- Document constructor parameters with `<param>`.
- Add `<remarks>` only when additional usage context is necessary.

### Delegates
- Add `<summary>`.
- Add `<param>` for delegate inputs and `<returns>` when applicable.

### Methods
- Add `<summary>` describing observable behavior.
- Add `<param>` for every argument.
- Add `<returns>` for non-`void` methods.
- Add `<exception>` when guard clauses or explicit throws exist.

### Properties / Fields / Events / Indexers
- Add `<summary>` explaining purpose and value semantics.
- For indexers, include `<param>` for index arguments.

## `cref` and Linking Rules

1. Prefer `<see cref="TypeName"/>` and `<seealso cref="TypeName"/>` for referenced symbols.
2. Use fully-qualified `cref` only when ambiguity exists.
3. Ensure `cref` targets resolve; avoid broken or speculative references.

## Quality Bar

Before finishing:
1. XML tags are well-formed.
2. Every documented parameter has exactly one matching `<param>` tag.
3. `<typeparam>` coverage is complete for generics.
4. Wording matches current code behavior and nullability semantics.
5. No placeholder text (`TODO`, `TBD`, `...`).

## Preferred Workflow

1. Locate missing-doc diagnostics (for example CS1591/CS1573/CS1711).
2. Document highest-scope symbols first (types, then members).
3. Add targeted `<param>`, `<returns>`, `<typeparam>`, and `<exception>` tags.
4. Rebuild and fix remaining documentation diagnostics.
5. Keep edits minimal and style-consistent with neighboring files.

## Anti-Patterns

- Copy/paste summaries that do not match behavior.
- Boilerplate that restates the symbol name only.
- Documenting private/internal members unless project standards require it.
- Adding remarks/examples without concrete value.

## Minimal Examples

```csharp
/// <summary>
/// Represents the supported boundary roles for volatility-based decomposition.
/// </summary>
public enum ComponentRole
{
    /// <summary>Unknown or not yet classified role.</summary>
    Unknown,

    /// <summary>Coordinates policy and orchestration workflows.</summary>
    Manager
}
```

```csharp
/// <summary>
/// Validates input values before processing.
/// </summary>
/// <param name="value">The input value to validate.</param>
/// <returns><see langword="true"/> when validation succeeds; otherwise, <see langword="false"/>.</returns>
/// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <see langword="null"/>.</exception>
public static bool Validate(string value)
{
    if (value is null)
    {
        throw new ArgumentNullException(nameof(value));
    }

    return value.Length > 0;
}
```
