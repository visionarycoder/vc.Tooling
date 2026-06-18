using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Distributed.Rules;

namespace VisionaryCoder.Analyzers.Distributed;

[DiagnosticAnalyzer(firstLanguage: LanguageNames.CSharp)]
public sealed class RepositoryAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> Rules =
    [
        new MissingAsyncRule(),
            new IQueryableLeakRule(),
            new DomainReturnTypeRule()
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
