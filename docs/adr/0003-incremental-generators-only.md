---
title: ADR-0003: Use Incremental Generators Only
description: Project documentation for ADR-0003: Use Incremental Generators Only.
status: active
updated: 2026-06-18
---
# ADR-0003: Use Incremental Generators Only

**Status:** Accepted

**Decision Date:** 2026-06-15

**Last Updated:** 2026-06-15

## Context

VisionaryCoder.Tooling uses Roslyn source generators extensively to reduce boilerplate and ensure consistency. However, the Roslyn generator API has evolved:

- **Legacy pattern** — Simple `ISourceGenerator` (deprecated in newer .NET)
- **Modern pattern** — `IIncrementalGenerator` (introduced in .NET 6, recommended)

Legacy generators have limitations:
- Less efficient — Re-run on every compilation
- Harder to debug — Opaque execution pipeline
- No caching — Wasteful for large codebases
- Harder to test — Less structured API

Incremental generators offer:
- Performance — Run only when relevant sources change
- Transparency — Explicit pipeline with step naming
- Caching — Efficient for large codebases
- Testability — Structured, testable API

We needed a decision to ensure consistency and future compatibility.

## Decision

**All source generators in VisionaryCoder.Tooling must use `IIncrementalGenerator` pattern.**

No legacy `ISourceGenerator` implementations are permitted.

### Implementation Requirements

Every generator must:

1. **Implement `IIncrementalGenerator`** — Not `ISourceGenerator`
2. **Use `.ForAttributeWithMetadataName()`** — For attribute-driven generation
3. **Define explicit pipeline stages** — Not nested callbacks
4. **Apply `.Where()` filters** — To skip irrelevant input
5. **Use `.Select()` transforms** — To extract metadata
6. **Register output** — Via `context.RegisterSourceOutput()`
7. **Generate deterministic code** — Same input always produces same output
8. **Emit diagnostics** — Via `VcGeneratorDiagnostics` for validation errors

### Attribute-Driven Generation

All generators scan for attributes:

```csharp
var provider = context.SyntaxProvider
    .ForAttributeWithMetadataName(
        "VisionaryCoder.Tooling.Shared.Attributes.MyAttribute",
        predicate: static (node, _) => node is ClassDeclarationSyntax,
        transform: static (ctx, _) => GetModel(ctx))
    .Where(static m => m is not null)
    .Select(static (m, _) => m!);

context.RegisterSourceOutput(provider, Execute);
```

## Consequences

### Positive
- **Performance** — Generators run only when relevant sources change
- **Transparency** — Explicit pipeline makes debugging easier
- **Consistency** — Single approach across all generators
- **Future-proof** — Aligns with Roslyn's recommended direction
- **Testability** — Structured API enables unit testing
- **Debugging** — Incremental debugging shows pipeline stages

### Negative
- **Learning curve** — Team must understand incremental generator pipeline
- **Refactoring effort** — Any legacy generators must be migrated
- **Complexity** — Pipeline-based approach is more verbose than simple generation

### Ongoing Investment
- Code reviews must validate incremental pattern
- New generators follow the pattern consistently
- Documentation guides developers on implementation

## Alternatives

### Alternative 1: Legacy ISourceGenerator
- **Description:** Use simple `ISourceGenerator` interface
- **Advantages:** Slightly less verbose
- **Disadvantages:** Less efficient, deprecated, harder to test
- **Why rejected:** Performance and future compatibility concerns

### Alternative 2: Mixed Approach
- **Description:** Use incremental for large generators, legacy for small ones
- **Advantages:** Flexibility
- **Disadvantages:** Inconsistency, cognitive load, harder to maintain
- **Why rejected:** Consistency is more important than minor convenience

### Alternative 3: Third-party Tool (e.g., Roslyn Analyzers, T4)
- **Description:** Use external code generation tools instead of Roslyn
- **Advantages:** May hide complexity
- **Disadvantages:** Adds external dependencies, less control, harder to integrate
- **Why rejected:** Roslyn generators are standard and integrated

## Related Decisions

- **Informed by:** [ADR-0001 — Volatility-Based Decomposition](0001-use-volatility-based-decomposition.md) (generators scaffold VBD contracts)
- **Related to:** [ADR-0002 — Dependency Injection Best Practices](0002-dependency-injection-best-practices.md) (generators scaffold DI registration)

## References

- **Microsoft Docs**, "Incremental Generators" — https://github.com/dotnet/roslyn/blob/main/docs/features/incremental-generators.md
- **Microsoft Docs**, "Source Generators" — https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview
- **VisionaryCoder.Tooling Generator Prompt** — See [.github/prompts/generator-prompt.md](.github/prompts/generator-prompt.md)

## Implementation

All generators follow the incremental pattern:

```csharp
[Generator(LanguageNames.CSharp)]
public sealed class MyGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var provider = context.SyntaxProvider
            .ForAttributeWithMetadataName(
                "VisionaryCoder.Tooling.Shared.Attributes.MyAttribute",
                predicate: static (node, _) => node is ClassDeclarationSyntax,
                transform: static (ctx, _) => GetModel(ctx))
            .Where(static m => m is not null)
            .Select(static (m, _) => m!);

        context.RegisterSourceOutput(provider, Execute);
    }

    private static MyModel? GetModel(GeneratorAttributeSyntaxContext ctx) { /* ... */ }
    private static void Execute(SourceProductionContext ctx, MyModel model) { /* ... */ }
}
```

## Questions?

See [.github/prompts/generator-prompt.md](.github/prompts/generator-prompt.md) for detailed implementation guidance.
