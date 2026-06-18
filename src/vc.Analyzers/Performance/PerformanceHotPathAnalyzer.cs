using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Performance.Rules;

namespace VisionaryCoder.Analyzers.Performance;

[DiagnosticAnalyzer(firstLanguage: LanguageNames.CSharp)]
public sealed class PerformanceHotPathAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> Rules =
        [new LinqInHotPathRule()];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [..Rules.Select(selector: rule => rule.Descriptor)];

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(analysisMode: GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        foreach (var rule in Rules)
        {
            rule.Register(context: context);
        }
    }
}