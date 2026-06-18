using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Design.Vbd.Rules;

namespace VisionaryCoder.Analyzers.Design.Vbd;

[DiagnosticAnalyzer(firstLanguage: LanguageNames.CSharp)]
public sealed class VbdManagerAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> Rules =
    [
        new VbdManagerUnstableContractRule(),
            new VbdManagerFeatureSpecificLogicRule(),
            new VbdManagerOrchestrationRule()
    ];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [..Rules.Select(selector: r => r.Descriptor)];

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



