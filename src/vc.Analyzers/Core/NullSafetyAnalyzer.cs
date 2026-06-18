using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Core.Rules;

namespace VisionaryCoder.Analyzers.Core;

[DiagnosticAnalyzer(firstLanguage: LanguageNames.CSharp)]
public sealed class NullSafetyAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> Rules =
        [new MissingNullCheckRule()];

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
