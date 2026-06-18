---
title: Code Fix Rule Composition Implementation Guide
description: Project documentation for Code Fix Rule Composition Implementation Guide.
status: active
updated: 2026-06-18
---
# Code Fix Rule Composition Implementation Guide

## Purpose

This guide standardizes code-fix implementation so each actionable analyzer rule has a matching, deterministic fix class while code-fix providers act as composition roots.

Use this guide for all new code fixes and when refactoring existing fix providers that contain multiple unrelated transformations.

## Target Architecture

- Code-fix provider: orchestration only.
- Fix classes: diagnostic IDs, registration, and transformation logic.
- Fix classes grouped under a local `Fixes` subfolder in the same category path as the provider.

## Folder and File Layout

```text
src/vc.CodeFixes/<Category>/
  <Concern>CodeFixProvider.cs
    Fixes/
    <RuleA>Fix.cs
    <RuleB>Fix.cs
    <RuleC>Fix.cs
```

Example for ApiDesign:

```text
src/vc.CodeFixes/Api/
  ApiDesignCodeFixProvider.cs
  Fixes/
    MissingApiControllerFix.cs
    NonRestfulNameFix.cs
    ApiVersioningFix.cs
    ApiValidationFix.cs
```

Example for Vbd:

```text
src/vc.CodeFixes/Design/Vbd/
  VbdEngineCodeFix.cs
  VbdManagerCodeFix.cs
  Fixes/
    VbdEngineInfrastructureAccessFix.cs
    VbdManagerOrchestrationFix.cs
```

## Step-by-Step

### 1. Define a Fix Contract

If the repository does not already have a shared contract, add one:

```csharp
namespace VisionaryCoder.Tooling.CodeFixes.Common;

public interface ICodeFixRule
{
    ImmutableArray<string> FixableDiagnosticIds { get; }
    void Register(CodeFixContext context);
}
```

### 2. Create the Fix Subfolder

Create or reuse a `Fixes` directory under the code-fix category folder.

Examples:

- `src/vc.CodeFixes/Api/Fixes/`
- `src/vc.CodeFixes/Security/Fixes/`
- `src/vc.CodeFixes/Design/Vbd/Fixes/`

### 3. Extract One Fix per Diagnostic

For each actionable analyzer diagnostic:

- Create a dedicated fix class.
- Bind the fix class to the matching diagnostic ID.
- Keep the fix deterministic.
- Prefer syntax-preserving edits.
- Use `SyntaxFactory` or narrowly scoped node replacement.
- Name the fix class after the semantic rule name, for example `MissingApiControllerFix` or `MissingNullCheckFix`.

Fix template:

```csharp
internal sealed class <RuleName>Fix : ICodeFixRule
{
    public ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.<SemanticIdName>);

    public void Register(CodeFixContext context)
    {
        foreach (var diagnostic in context.Diagnostics)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    "<Action title>",
                    cancellationToken => ApplyAsync(context.Document, diagnostic, cancellationToken),
                    equivalenceKey: "<Action title>"),
                diagnostic);
        }
    }

    private static Task<Document> ApplyAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        // deterministic fix logic
        return Task.FromResult(document);
    }
}
```

### 4. Simplify the Provider to Composition

The provider should:

- Compose all fix classes.
- Register the matching code fixes.
- Expose the union of all fixable diagnostic IDs.
- Use the default Fix All provider unless a rule needs something custom.

Provider template:

```csharp
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ApiDesignCodeFixProvider))]
[Shared]
public sealed class ApiDesignCodeFixProvider : CodeFixProvider
{
    private static readonly ImmutableArray<ICodeFixRule> _fixes =
        ImmutableArray.Create<ICodeFixRule>(
            new MissingApiControllerFix(),
            new NonRestfulNameFix());

    public override ImmutableArray<string> FixableDiagnosticIds =>
        _fixes.SelectMany(fix => fix.FixableDiagnosticIds).ToImmutableArray();

    public override FixAllProvider GetFixAllProvider() =>
        WellKnownFixAllProviders.BatchFixer;

    public override Task RegisterCodeFixesAsync(CodeFixContext context)
    {
        foreach (var fix in _fixes)
        {
            fix.Register(context);
        }

        return Task.CompletedTask;
    }
}
```

### 5. Validate Class/File Conventions

- One top-level class per file.
- File name must match class name.
- Fix class names end with `Fix`.
- Provider class name should match the category and concern.
- Fix namespace should match the provider namespace plus `.Fixes`.

### 6. Validate Diagnostic IDs and Build

- Ensure all fixable IDs come from `DiagnosticIds`.
- Ensure each code fix maps to exactly one analyzer rule, unless the transformation is intentionally shared.
- Validate deterministic output.
- Build:

```bash
dotnet build src/vc.CodeFixes/vc.CodeFixes.csproj
```

## Code-Fix Mapping Guidance

The strongest matches are typically:

- API diagnostics -> add annotations, adjust names, add metadata.
- Async diagnostics -> add `Async` suffix, add `CancellationToken`, replace blocking calls.
- Security diagnostics -> replace obvious unsafe patterns with safe equivalents.
- Null-safety diagnostics -> add `ThrowIfNull` or null guards.
- Exception-safety diagnostics -> convert `async void`, add logging, narrow catch blocks.
- Resilience diagnostics -> wrap calls with retry/timeout/circuit-breaker patterns.
- Performance diagnostics -> replace obvious hot-path allocations with direct loops or simpler constructs.

## Design Constraints

1. Fixes must be deterministic.
2. Fixes should preserve behavior unless the analyzer rule explicitly targets a behavioral issue.
3. Fixes should avoid broad refactors.
4. If multiple valid transformations exist, prefer the least invasive one.
5. Shared helpers should move to common utilities only when reused by 2+ fix classes.

## Suggested First Fix Set

If you are starting from the current code-fix footprint, prioritize:

- `MissingApiControllerFix`
- `NonRestfulNameFix`
- `ApiVersioningFix`
- `ApiValidationFix`
- `MissingAsyncSuffixFix`
- `MissingCancellationTokenFix`
- `MissingNullCheckFix`
- `MissingAuthorizationFix`
- `HardcodedSecretFix`

These provide the quickest user value and map directly to the most actionable diagnostics.

## Related Guidance

- `.agents/skills/codefix-rule-composition/SKILL.md`
- `.agents/skills/analyzer-rule-composition/SKILL.md`
- `docs/codefixes-plan.md`
- `docs/instructions/unified-diagnostic-ids.md`
- `docs/instructions/analyzer-rule-composition.md`
- `.agents/skills/class-file-conventions/SKILL.md`