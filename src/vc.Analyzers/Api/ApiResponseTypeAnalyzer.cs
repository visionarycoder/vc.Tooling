using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;
using Vc.Analyzers.Api.Rules;

namespace Vc.Analyzers.Api;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ApiResponseTypeAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> Rules =
        ImmutableArray.Create<IAnalyzerRule>(new ApiResponseTypeRule());

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


