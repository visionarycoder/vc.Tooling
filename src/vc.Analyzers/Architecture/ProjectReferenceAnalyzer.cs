using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Architecture.Rules;

namespace VisionaryCoder.Analyzers.Architecture;

[DiagnosticAnalyzer(firstLanguage: LanguageNames.CSharp)]
public sealed class ProjectReferenceAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> Rules =
        [new ProjectReferenceViolationRule()];

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
