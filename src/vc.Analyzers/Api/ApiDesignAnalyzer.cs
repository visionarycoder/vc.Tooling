using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Api.Rules;

namespace VisionaryCoder.Analyzers.Api;

[DiagnosticAnalyzer(firstLanguage: LanguageNames.CSharp)]
public sealed class ApiDesignAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> Rules =
    [
        new MissingApiControllerRule(),
        new NonRestfulNameRule()
    ];

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        [.. Rules.Select<IAnalyzerRule, DiagnosticDescriptor>(selector: rule => rule.Descriptor)];

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