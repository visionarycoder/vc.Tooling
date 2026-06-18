using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Design.Rules;

namespace VisionaryCoder.Analyzers.Design;

[DiagnosticAnalyzer(firstLanguage: LanguageNames.CSharp)]
public sealed class ImmutabilityAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> Rules =
        [new MutableRecordFieldRule()];

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
