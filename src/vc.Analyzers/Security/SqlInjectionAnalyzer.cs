using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;
using Vc.Analyzers.Security.Rules;

namespace Vc.Analyzers.Security;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SqlInjectionAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> Rules =
        ImmutableArray.Create<IAnalyzerRule>(new SqlInjectionRule());

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