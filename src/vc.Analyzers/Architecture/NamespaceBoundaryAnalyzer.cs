using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;
using Vc.Analyzers.Architecture.Rules;

namespace Vc.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NamespaceBoundaryAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> Rules =
        ImmutableArray.Create<IAnalyzerRule>(new NamespaceBoundaryViolationRule());

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        Rules.Select(rule => rule.Descriptor).ToImmutableArray();

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        foreach (var rule in Rules)
        {
            rule.Register(context);
        }
    }
}
