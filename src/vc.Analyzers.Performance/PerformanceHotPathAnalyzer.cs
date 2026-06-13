namespace Vc.Analyzers.Performance;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class PerformanceHotPathAnalyzer : DiagnosticAnalyzer
{
    public const string LinqInHotPathId = "VCPERF001";

    private static readonly DiagnosticDescriptor LinqInHotPathRule = new(
        id: LinqInHotPathId,
        title: "Avoid LINQ in hot paths",
        messageFormat: "Consider avoiding LINQ call '{0}' in performance-critical code.",
        category: "Performance",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(LinqInHotPathRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TODO:
        // - Register syntax node action for InvocationExpression.
        // - Detect LINQ method calls inside methods marked with a [HotPath] attribute (to be defined) or known performance-critical contexts.
    }
}
