using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;
using Vc.Analyzers.Design.Vbd.Rules;

namespace Vc.Analyzers.Design.Vbd;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class VbdEngineAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> Rules =
        ImmutableArray.Create<IAnalyzerRule>(
            new VbdEngineInfrastructureAccessRule(),
            new VbdEngineNondeterminismRule(),
            new VbdEngineStateMutationRule());

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        Rules.Select(r => r.Descriptor).ToImmutableArray();

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



