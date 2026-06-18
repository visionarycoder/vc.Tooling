using VisionaryCoder.Analyzers.Abstractions;
using VisionaryCoder.Analyzers.Core.Rules;

namespace VisionaryCoder.Analyzers.Core;

[DiagnosticAnalyzer(firstLanguage: LanguageNames.CSharp)]
public sealed class AsyncCorrectnessAnalyzer : DiagnosticAnalyzer
{
    private static readonly ImmutableArray<IAnalyzerRule> Rules =
    [
        new BlockingCallRule(),
            new FireAndForgetRule(),
            new MissingAsyncSuffixRule()
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
