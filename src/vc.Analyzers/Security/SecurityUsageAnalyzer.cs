using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Security.Rules;

namespace VisionaryCoder.Analyzers.Security;

[DiagnosticAnalyzer(firstLanguage: LanguageNames.CSharp)]
public sealed class SecurityUsageAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> Rules =
        [new SecurityUsageInputValidationRule()];

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



