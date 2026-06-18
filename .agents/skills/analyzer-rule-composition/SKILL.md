---
title: Analyzer Rule Composition Skill
description: Project documentation for Analyzer Rule Composition Skill.
status: active
updated: 2026-06-18
---
# Analyzer Rule Composition Skill

## Overview

This skill defines the canonical analyzer structure for VisionaryCoder.Tooling:

- One analyzer class orchestrates analysis.
- Each diagnostic rule is implemented in its own class.
- Rule classes live in a category-local `Rules` subfolder next to analyzers in that folder.

This separates policy/rule definitions from analyzer wiring and improves testability, composability, and maintainability.

## Rule Extraction Rules

### ARC0001: One Rule Class per Diagnostic Rule
Each diagnostic rule must be implemented as a dedicated class file.

- No inline `DiagnosticDescriptor` blocks in analyzer classes.
- No analyzer class owning multiple rule descriptors directly.
- One file, one top-level rule class.

### ARC0002: Analyzer as Composition Root
Analyzer classes must:

- Compose one or more rule classes.
- Register rule callbacks in `Initialize`.
- Expose `SupportedDiagnostics` as union of composed rule descriptors.
- Avoid embedding rule-specific syntax/semantic logic.

### ARC0003: Rule Subfolder Required
Rules must live in a `Rules` subfolder under the analyzer's current category folder.

Recommended layout:

```text
src/vc.Analyzers/<Category>/
  <AnalyzerName>.cs
    Rules/
    <RuleName>Rule.cs
    <RuleName>Rule.cs
```

For nested category paths, keep the `Rules` folder local to that path:

```text
src/vc.Analyzers/Design/Vbd/
    VbdAccessAnalyzer.cs
    VbdEngineAnalyzer.cs
    VbdManagerAnalyzer.cs
    Rules/
        VbdAccessBoundaryViolationRule.cs
        VbdEngineNondeterminismRule.cs
        VbdManagerOrchestrationRule.cs
```

Example:

```text
src/vc.Analyzers/Api/
    ApiDesignAnalyzer.cs
    Rules/
        MissingApiControllerRule.cs
        NonRestfulNameRule.cs
```

Another example:

```text
src/vc.Analyzers/Design/
    ExceptionSafetyAnalyzer.cs
    Rules/
    EmptyCatchRule.cs
    BroadCatchRule.cs
    SwallowedExceptionRule.cs
        AsyncVoidRule.cs
```

### ARC0004: Rule Class Contract
Each rule class should expose:

- `DiagnosticDescriptor Descriptor`
- `void Register(AnalysisContext context)`

Preferred interface:

```csharp
public interface IAnalyzerRule
{
    DiagnosticDescriptor Descriptor { get; }
    void Register(AnalysisContext context);
}
```

Rule class skeleton:

```csharp
internal sealed class EmptyCatchRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => _descriptor;

    private static readonly DiagnosticDescriptor _descriptor = new(
        id: DiagnosticIds.DesignExceptionSafetyEmptyCatch,
        title: "Empty catch block",
        messageFormat: "Catch block does not handle or rethrow the exception.",
        category: "ExceptionSafety",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.CatchClause);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        // rule-specific logic
    }
}
```

### ARC0005: Unified Diagnostic IDs Remain Mandatory
Every rule descriptor ID must come from `DiagnosticIds` constants.

- Never use magic string IDs.
- Rule class names and ID names should be semantically aligned.

### ARC0006: Rule Names Must Match Analyzer Rule Names
When an analyzer conceptually exposes named rules, the extracted rule class and file name must use that exact semantic rule name.

- Prefer `MissingApiControllerRule` over analyzer-prefixed variants like `ApiDesignMissingApiControllerRule` when the analyzer already refers to that rule by name.
- Keep analyzer composition arrays and any descriptor-oriented naming aligned with the extracted rule class names.
- Rule namespace should match the analyzer folder path plus `.Rules`.

## Analyzer Composition Example

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ExceptionSafetyAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> _rules =
        ImmutableArray.Create<IAnalyzerRule>(
            new EmptyCatchRule(),
            new BroadCatchRule(),
            new SwallowedExceptionRule(),
            new AsyncVoidExceptionHandlerRule());

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

## Naming Conventions

- Analyzer class: `<Concern>Analyzer`
- Rule folder: `Rules`
- Rule namespace: `<AnalyzerNamespace>.Rules`
- Rule class: exact semantic rule name ending in `Rule`
- Rule file name must match class name (class-file convention)

## Migration Guidance

When migrating an existing analyzer with multiple rules:

1. Create or reuse a local `Rules` subfolder in the analyzer's folder.
2. Move each `DiagnosticDescriptor` + analysis callback into its own rule class.
3. Keep analyzer class focused on composition and registration.
4. Replace old `SupportedDiagnostics` implementation with descriptor union from rules.
5. Validate analyzer behavior and diagnostic IDs.

## Benefits

- Lower complexity per file.
- Easier per-rule unit testing.
- Cleaner analyzer orchestration.
- Safer parallel rule development.
- Better long-term extensibility.

## Related Resources

- `.agents/skills/unified-diagnostic-ids/SKILL.md`
- `docs/instructions/unified-diagnostic-ids.md`
- `docs/instructions/analyzer-rule-composition.md`
