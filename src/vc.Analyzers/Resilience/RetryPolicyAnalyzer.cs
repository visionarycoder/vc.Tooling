using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Resilience.Rules;

namespace VisionaryCoder.Analyzers.Resilience;

[DiagnosticAnalyzer(firstLanguage: LanguageNames.CSharp)]
public sealed class RetryPolicyAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> Rules =
    [
        new MissingRetryPolicyRule(),
            new UnusedRetryPolicyRule(),
            new ExcessiveRetryCountRule()
    ];

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
