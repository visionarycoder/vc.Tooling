using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Api.Rules;

namespace VisionaryCoder.Analyzers.Api;

[DiagnosticAnalyzer(firstLanguage: LanguageNames.CSharp)]
public sealed class ApiResponseTypeAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> Rules =
        [new ApiResponseTypeRule()];

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


