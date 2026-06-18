using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Security.Rules;

namespace VisionaryCoder.Analyzers.Security;

[DiagnosticAnalyzer(firstLanguage: LanguageNames.CSharp)]
public sealed class MissingAuthorizationAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> Rules =
        [new MissingAuthorizationRule()];

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