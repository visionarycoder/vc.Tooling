using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;
using Vc.Analyzers.Resilience.Rules;

namespace Vc.Analyzers.Resilience;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TimeoutAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> Rules =
        ImmutableArray.Create<IAnalyzerRule>(
            new MissingTimeoutRule(),
            new UnusedTimeoutRule(),
            new ExcessiveTimeoutRule());

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
