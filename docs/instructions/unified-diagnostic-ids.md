---
title: Unified Diagnostic IDs Implementation Guide
description: Project documentation for Unified Diagnostic IDs Implementation Guide.
status: active
updated: 2026-06-18
---
# Unified Diagnostic IDs Implementation Guide

## Overview

This guide provides step-by-step instructions for implementing or updating analyzers using the unified diagnostic ID system. The system replaces magic string IDs with centralized semantic constants, improving maintainability and consistency.

## When to Use This Guide

Use this guide when:
- Creating a new analyzer
- Updating an existing analyzer to use centralized diagnostic IDs
- Adding new diagnostic rules to an existing analyzer
- Migrating legacy analyzers to the unified system

For analyzer structure (rule-per-class composition), also use:
- `docs/instructions/analyzer-rule-composition.md`

## Implementation Steps

### Step 1: Plan Your Diagnostic IDs

Before writing code, determine:
1. Which category your diagnostics belong to (see [Category Registry](#category-registry))
2. How many diagnostic IDs you need
3. Semantic names for each ID

**Example:** Building an `AllocationAnalyzer`
- Category: Performance (VCPERF)
- IDs needed: 1 (boxing allocations)
- Constant name: `PerfBoxingAllocation`
- Full ID: `VCPERF0002`

### Step 2: Add Constants to DiagnosticIds.cs

Locate `src/vc.Analyzers/Common/DiagnosticIds.cs` and add your constants in the appropriate category section.

**Pattern:**
```csharp
public const string {SemanticName} = "{VCCATEGORYNNNN}";
```

**Example:**
```csharp
// Performance Category (VCPERF)
public const string PerfLinqInHotPath = "VCPERF0001";
public const string PerfBoxingAllocation = "VCPERF0002";  // NEW
```

**Guidelines:**
- Add XML comments explaining the rule:
  ```csharp
  /// <summary>
  /// Boxing detected in marked hot path. Use generic alternatives.
  /// </summary>
  public const string PerfBoxingAllocation = "VCPERF0002";
  ```
- Keep IDs sequential within each category
- Use semantic names that describe the violation

### Step 3: Create or Update Rule Class(es) and Analyzer Composition

In your analyzer file (e.g., `AllocationAnalyzer.cs`):

**3a. Add required imports:**
```csharp
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Performance;
```

**3b. For analyzers with multiple rules, create one rule class per diagnostic in a `<Concern>Rules` subfolder.**

Rule class example:
```csharp
internal sealed class BoxingAllocationRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => _descriptor;

    private static readonly DiagnosticDescriptor _descriptor = new(
        id: DiagnosticIds.PerfBoxingAllocation,
        title: "Boxing detected in hot path",
        messageFormat: "Value type '{0}' is boxed. Use generic alternatives.",
        category: "Performance",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.CastExpression);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        // rule logic
    }
}
```

**3c. Keep analyzer class focused on composition:**
```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AllocationAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> _rules =
        ImmutableArray.Create<IAnalyzerRule>(new BoxingAllocationRule());

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        _rules.Select(r => r.Descriptor).ToImmutableArray();

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        foreach (var rule in _rules)
        {
            rule.Register(context);
        }
    }
}
```

### Step 4: Define DiagnosticDescriptors in Rule Classes

Reference `DiagnosticIds` constants when creating `DiagnosticDescriptor` instances in each rule class:

```csharp
private static readonly DiagnosticDescriptor _descriptor = new(
    id: DiagnosticIds.PerfBoxingAllocation,  // Use the constant, not magic string
    title: "Boxing detected in hot path",
    messageFormat: "Value type '{0}' is boxed. Use generic alternatives or struct patterns.",
    category: "Performance",
    defaultSeverity: DiagnosticSeverity.Warning,
    isEnabledByDefault: true);
```

✅ **DO** reference constants
❌ **DON'T** use hardcoded strings like `"VCPERF0002"`

### Step 5: Implement Analysis Logic in Rule Classes

Implement analysis callbacks in rule classes and register through `Register(AnalysisContext context)`.

```csharp
public void Register(AnalysisContext context)
{
    context.RegisterSyntaxNodeAction(AnalyzeCastExpression, SyntaxKind.CastExpression);
}
```

### Step 6: Update DIAGNOSTIC_IDS_REFERENCE.md

If adding a new category, document it in `src/vc.Analyzers/Common/DIAGNOSTIC_IDS_REFERENCE.md`:

**Format:**
```markdown
### Category Name (VCCATEGORYCODE0001-00NN)
- `VCCATEGORYCODE0001` — Rule description
- `VCCATEGORYCODE0002` — Another rule
```

**Example (Performance expansion):**
```markdown
### Performance (VCPERF0001-0007)
- `VCPERF0001` — LINQ in hot path
- `VCPERF0002` — Boxing allocation in hot path  ← NEW
- `VCPERF0003` — Regex recompilation
```

### Step 7: Verify Compilation

Test that your analyzer compiles without errors:

```bash
dotnet build src/vc.Analyzers/vc.Analyzers.csproj
```

All analyzers should compile with no magic string warnings or DiagnosticIds reference errors.

## Category Registry

Use this table to choose the right category for your analyzer:

| Code | Category | Purpose | Example Rules |
|------|----------|---------|---|
| VCARCH | Architecture | Boundaries, layering, structure | Cyclic dependencies, layering violations |
| VCAPI | API Design | HTTP/REST conventions | Missing controllers, non-RESTful names |
| VCDTO | DTO Design | Data transfer object rules | Mutable state, circular references |
| VCMAP | Mapping | Object mapping validation | Unmapped properties |
| VCASYNC | Async Correctness | async/await patterns | Async void, blocking calls, fire-and-forget |
| VCDESIGN | Design Principles | General design rules | Exception safety, immutability |
| VCVBDA | VBD Access Vault | Access layer boundaries | Infrastructure leakage |
| VCVBDE | VBD Engine Vault | Engine layer boundaries | Non-determinism, state issues |
| VCVBDM | VBD Manager Vault | Manager layer boundaries | Unstable contracts |
| VCDIST | Distributed | Event sourcing, messaging | Missing Apply, IQueryable leakage |
| VCPERF | Performance | Hot paths, optimizations | LINQ overhead, boxing, allocations |
| VCRES | Resilience | Circuit breakers, retries | Missing policies, excessive timeouts |
| VCSEC | Security | Security vulnerabilities | SQL injection, hardcoded secrets |
| VCNULL | Null Safety | Null handling | Missing null checks |
| VCDOC | Documentation | XML comments | Missing XML docs |
| VCNAMING | Naming | Naming conventions | Convention violations |

## Example: Complete Migration

### Before (Magic Strings)
```csharp
namespace Vc.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LayeringAnalyzer : DiagnosticAnalyzer
{
    private static readonly DiagnosticDescriptor LayeringViolationRule = new(
        id: "VCARCH001",  // ❌ Magic string
        title: "Layering violation",
        messageFormat: "Component '{0}' violates layering rules.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);
    
    public override void Initialize(AnalysisContext context) { }
}
```

### After (Unified Constants + Rule Composition)
```csharp
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LayeringAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> _rules =
        ImmutableArray.Create<IAnalyzerRule>(new LayeringViolationRule());

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        _rules.Select(r => r.Descriptor).ToImmutableArray();

    public override void Initialize(AnalysisContext context)
    {
        foreach (var rule in _rules)
        {
            rule.Register(context);
        }
    }
}
```

## Bulk Migration (Multiple Analyzers)

To migrate multiple analyzers at once:

1. **Add all constants** to DiagnosticIds.cs in one edit
2. **Add imports** to each analyzer file (batch edit if possible)
3. **Replace magic strings** with constants in each analyzer
4. **Test compilation** for all files
5. **Update documentation** (DIAGNOSTIC_IDS_REFERENCE.md)

## Troubleshooting

### "The type or namespace name 'DiagnosticIds' could not be found"

**Solution:** Ensure your analyzer imports `VisionaryCoder.Tooling.Analyzers.Common`:
```csharp
using VisionaryCoder.Tooling.Analyzers.Common;
```

### "Could not find member 'SomeConstantName' in DiagnosticIds"

**Solution:** Check spelling and PascalCase naming in DiagnosticIds.cs. Example: `PerfLinqInHotPath`, not `PerfLINQInHotPath`.

### Build fails with "netstandard2.0 required"

**Solution:** Ensure analyzer project targets netstandard2.0 in .csproj:
```xml
<TargetFramework>netstandard2.0</TargetFramework>
```

### Constants don't resolve in editor

**Solution:** Reload the VS Code window or restart the language server. Missing IntelliSense is usually a transient issue.

## Best Practices

1. **Be specific** — Use semantic names that describe the violation
   - ✅ `ResilienceRetryPolicyMissing` 
   - ❌ `RetryError`

2. **Group by concern** — Related rules share a prefix
   - `DistributedRepositoryAsyncMissing`
   - `DistributedRepositoryQueryableLeakage`
   - `DistributedRepositoryContractViolation`

3. **Document non-obvious rules** — Add XML comments in DiagnosticIds.cs

4. **Reserve ID ranges** — Allocate contiguous ranges per category to avoid conflicts
   - VCPERF: 0001-0007
   - VCRES: 0001-0012
   - etc.

5. **Keep DiagnosticIds.cs organized** — Group by category with section comments

6. **Update documentation** — Keep DIAGNOSTIC_IDS_REFERENCE.md in sync

## See Also

- [Unified Diagnostic IDs Skill](.agents/skills/unified-diagnostic-ids/SKILL.md)
- [Analyzer Rule Composition Guide](analyzer-rule-composition.md)
- `src/vc.Analyzers/Common/DiagnosticIds.cs` — Complete constant definitions
- `src/vc.Analyzers/Common/DIAGNOSTIC_IDS_REFERENCE.md` — Reference for users
- Analyzer examples: AsyncVoidAnalyzer.cs, ExceptionSafetyAnalyzer.cs, etc.
