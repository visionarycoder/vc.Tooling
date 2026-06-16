# Code Fix Rule Composition Skill

## Overview

This skill defines the canonical code-fix structure for VisionaryCoder.Tooling.

- One code-fix provider acts as the composition root.
- Each actionable analyzer rule gets a matching code-fix class.
- Code-fix classes live in a category-local `Fixes` subfolder next to the related analyzer category.

The goal is to keep fixes deterministic, testable, and aligned with the analyzer rules they support.

## Core Rules

### CFR0001: One Fix Class per Analyzer Rule

If an analyzer rule is actionable, create a dedicated code-fix class for it.

- Do not place unrelated fix logic in a single provider.
- Do not make the provider perform the actual syntax rewrite.
- One file, one top-level fix class.

### CFR0002: Provider as Composition Root

The code-fix provider must:

- Compose all matching fix classes.
- Expose the union of all fixable diagnostic IDs.
- Register code fixes by delegating to the matching fix class.
- Avoid embedding transformation logic.

### CFR0003: Local Fix Folder Required

Code-fix classes must live in a `Fixes` subfolder under the relevant category folder.

Recommended layout:

```text
src/vc.CodeFixes/<Category>/
  <Concern>CodeFixProvider.cs
    Fixes/
    <RuleName>Fix.cs
    <RuleName>Fix.cs
```

Example:

```text
src/vc.CodeFixes/Api/
  ApiDesignCodeFixProvider.cs
  Fixes/
    MissingApiControllerFix.cs
    NonRestfulNameFix.cs
```

Example with nested category paths:

```text
src/vc.CodeFixes/Design/Vbd/
  VbdEngineCodeFix.cs
  VbdManagerCodeFix.cs
  Fixes/
    VbdEngineInfrastructureAccessFix.cs
    VbdManagerOrchestrationFix.cs
```

### CFR0004: Fix Class Contract

Each fix class should expose:

- A diagnostic identifier or set of identifiers it fixes.
- A deterministic `RegisterCodeFixes` entry point.
- A transformation method that returns the updated `Document`.

Preferred interface:

```csharp
public interface ICodeFixRule
{
    ImmutableArray<string> FixableDiagnosticIds { get; }
    void Register(CodeFixContext context);
}
```

### CFR0005: Deterministic Transformations Only

Code fixes must be safe and predictable.

- Prefer `SyntaxFactory` or precise node replacement.
- Preserve trivia where reasonable.
- Avoid semantic rewrites that can alter behavior unless the analyzer explicitly calls for that behavior.
- Prefer a single, clear fix per diagnostic.

### CFR0006: Matching Rule Names

The fix class name should match the analyzer rule it addresses.

- `MissingApiControllerRule` -> `MissingApiControllerFix`
- `NonRestfulNameRule` -> `NonRestfulNameFix`
- `MissingNullCheckRule` -> `MissingNullCheckFix`

The provider name should remain focused on the category or concern, not the individual rule.

## Provider Example

```csharp
[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(ApiDesignCodeFixProvider))]
[Shared]
public sealed class ApiDesignCodeFixProvider : CodeFixProvider
{
    private static readonly ImmutableArray<ICodeFixRule> _fixes =
        ImmutableArray.Create<ICodeFixRule>(
            new MissingApiControllerFix(),
            new NonRestfulNameFix(),
            new ApiVersioningFix(),
            new ApiValidationFix());

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

## Fix Class Example

```csharp
internal sealed class MissingApiControllerFix : ICodeFixRule
{
    public ImmutableArray<string> FixableDiagnosticIds =>
        ImmutableArray.Create(DiagnosticIds.ApiDesignControllerMissing);

    public void Register(CodeFixContext context)
    {
        foreach (var diagnostic in context.Diagnostics)
        {
            context.RegisterCodeFix(
                CodeAction.Create(
                    "Add [ApiController]",
                    cancellationToken => ApplyAsync(context.Document, diagnostic, cancellationToken),
                    equivalenceKey: "Add [ApiController]"),
                diagnostic);
        }
    }

    private static Task<Document> ApplyAsync(Document document, Diagnostic diagnostic, CancellationToken cancellationToken)
    {
        // deterministic syntax transformation
        return Task.FromResult(document);
    }
}
```

## Design Constraints

1. A fix must directly correspond to an analyzer diagnostic.
2. The fix should be as local as possible.
3. If multiple transformations are possible, prefer the least invasive one.
4. If a safe fix cannot be guaranteed, provide a suggestion or light-touch code action instead of a rewrite.
5. Shared helper code should move to common utilities only when reused by multiple fixes.

## Validation Checklist

- Each actionable analyzer rule has a matching code fix.
- Fixable diagnostic IDs are centralized and explicit.
- Fixes preserve formatting and trivia where practical.
- The provider remains orchestration-only.
- Category and folder names mirror the analyzer structure.

## Related Resources

- `.agents/skills/analyzer-rule-composition/SKILL.md`
- `docs/instructions/analyzer-rule-composition.md`
- `docs/codefixes-plan.md`
- `docs/instructions/unified-diagnostic-ids.md`
- `.agents/skills/class-file-conventions/SKILL.md`