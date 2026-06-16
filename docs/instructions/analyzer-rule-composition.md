# Analyzer Rule Composition Implementation Guide

## Purpose

This guide standardizes analyzer implementation so each diagnostic rule is isolated in its own class while analyzers act as composition roots.

Use this guide for all new analyzers and when refactoring existing analyzers that contain multiple rules.

## Target Architecture

- Analyzer class: orchestration only.
- Rule classes: diagnostic descriptor + registration + analysis logic.
- Rule classes grouped under a local `Rules` subfolder in the same category path as the analyzer.

## Folder and File Layout

```text
src/vc.Analyzers/<Category>/
  <Concern>Analyzer.cs
    Rules/
    <RuleA>Rule.cs
    <RuleB>Rule.cs
    <RuleC>Rule.cs
```

Example for ExceptionSafety:

```text
src/vc.Analyzers/Design/
  ExceptionSafetyAnalyzer.cs
    Rules/
    EmptyCatchRule.cs
    BroadCatchRule.cs
    SwallowedExceptionRule.cs
        AsyncVoidRule.cs
```

Example for ApiDesign:

```text
src/vc.Analyzers/Api/
    ApiDesignAnalyzer.cs
    Rules/
        MissingApiControllerRule.cs
        NonRestfulNameRule.cs
```

## Step-by-Step

### 1. Define Rule Contracts

Add shared abstractions if not present:

```csharp
namespace VisionaryCoder.Tooling.Analyzers.Common;

public interface IAnalyzerRule
{
    DiagnosticDescriptor Descriptor { get; }
    void Register(AnalysisContext context);
}
```

### 2. Create Rule Subfolder

Create or reuse a `Rules` directory under the analyzer's current category folder.

Examples:

- `src/vc.Analyzers/Api/Rules/`
- `src/vc.Analyzers/Security/Rules/`
- `src/vc.Analyzers/Design/Vbd/Rules/`

### 3. Extract One Rule per Class

For each rule currently inside an analyzer:

- Move `DiagnosticDescriptor` to a dedicated rule class.
- Move the corresponding callback and helper methods into that class.
- Keep only rule-specific code in that file.
- Use `DiagnosticIds` for descriptor IDs.
- Name the extracted rule class after the semantic rule name used by the analyzer or descriptor, for example `MissingApiControllerRule` or `NonRestfulNameRule`.

Rule template:

```csharp
internal sealed class <RuleName>Rule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => _descriptor;

    private static readonly DiagnosticDescriptor _descriptor = new(
        id: DiagnosticIds.<SemanticIdName>,
        title: "...",
        messageFormat: "...",
        category: "...",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind....);
    }

    private static void Analyze(SyntaxNodeAnalysisContext context)
    {
        // rule logic
    }
}
```

### 4. Simplify Analyzer Class to Composition

Analyzer class should:

- Instantiate all rule classes.
- Register all rules in `Initialize`.
- Return `SupportedDiagnostics` from the composed rule descriptors.

Analyzer template:

```csharp
[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class <Concern>Analyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> _rules =
        ImmutableArray.Create<IAnalyzerRule>(
            new <RuleA>Rule(),
            new <RuleB>Rule());

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

### 5. Validate Class/File Conventions

- One top-level class per file.
- File name must match class name.
- Rule class names end with `Rule`.
- Rule namespace should match the analyzer namespace plus `.Rules`.

### 6. Validate Diagnostics and Build

- Ensure all IDs come from `DiagnosticIds`.
- Ensure analyzer exposes all rule descriptors in `SupportedDiagnostics`.
- Build:

```bash
dotnet build src/vc.Analyzers/vc.Analyzers.csproj
```

## ExceptionSafety Migration Example

Current state:

- One analyzer with 4 rules embedded.

Target state:

- `ExceptionSafetyAnalyzer.cs` (composition only)
- `Rules/EmptyCatchRule.cs`
- `Rules/BroadCatchRule.cs`
- `Rules/SwallowedExceptionRule.cs`
- `Rules/AsyncVoidRule.cs`

## Design Constraints

1. Rule classes must not depend on analyzer internals.
2. Analyzer must not duplicate rule logic.
3. Rule-level helpers should remain local to each rule class unless shared.
4. Shared helper code should move to common utility classes only when reused by 2+ rules.
5. Keep rule constructors parameterless unless dependency injection is explicitly introduced.

## Related Guidance

- `.agents/skills/analyzer-rule-composition/SKILL.md`
- `.agents/skills/unified-diagnostic-ids/SKILL.md`
- `docs/instructions/unified-diagnostic-ids.md`
- `.agents/skills/class-file-conventions/SKILL.md`
