namespace Vc.Analyzers.Core;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AsyncCorrectnessAnalyzer : DiagnosticAnalyzer
{
    public const string BlockingCallId = "VCASYNC001";
    public const string FireAndForgetId = "VCASYNC002";
    public const string MissingAsyncSuffixId = "VCASYNC003";

    private static readonly DiagnosticDescriptor BlockingCallRule = new(
        id: BlockingCallId,
        title: "Avoid blocking calls in async methods",
        messageFormat: "Avoid blocking call '{0}' inside an async method; use await instead.",
        category: "AsyncCorrectness",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor FireAndForgetRule = new(
        id: FireAndForgetId,
        title: "Avoid fire-and-forget tasks",
        messageFormat: "Task-returning call '{0}' is not awaited or observed.",
        category: "AsyncCorrectness",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MissingAsyncSuffixRule = new(
        id: MissingAsyncSuffixId,
        title: "Async method should end with 'Async'",
        messageFormat: "Async method '{0}' should be suffixed with 'Async'.",
        category: "AsyncCorrectness",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(BlockingCallRule, FireAndForgetRule, MissingAsyncSuffixRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TODO: Register syntax node actions for:
        // - MethodDeclaration: detect async methods without Async suffix.
        // - InvocationExpression: detect Task.Result / Wait / GetAwaiter().GetResult() inside async methods.
        // - InvocationExpression: detect Task-returning calls whose result is ignored (fire-and-forget).
    }
}
