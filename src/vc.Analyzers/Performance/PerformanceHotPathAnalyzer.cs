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

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AllocationAnalyzer : DiagnosticAnalyzer
{
    public const string AllocationInHotPathId = "VCPERF002";

    private static readonly DiagnosticDescriptor AllocationInHotPathRule = new(
        id: AllocationInHotPathId,
        title: "Avoid allocations in hot paths",
        messageFormat: "Consider avoiding allocation of '{0}' in performance-critical code.",
        category: "Performance",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(AllocationInHotPathRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TODO:
        // - Register syntax node action for ObjectCreationExpression.
        // - Detect allocations inside methods marked with a [HotPath] attribute (to be defined) or known performance-critical contexts.
    }
}

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class BoxingAnalyzer : DiagnosticAnalyzer
{
    public const string BoxingInHotPathId = "VCPERF003";

    private static readonly DiagnosticDescriptor BoxingInHotPathRule = new(
        id: BoxingInHotPathId,
        title: "Avoid boxing in hot paths",
        messageFormat: "Consider avoiding boxing of '{0}' in performance-critical code.",
        category: "Performance",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(BoxingInHotPathRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TODO:
        // - Register syntax node action for BoxingExpression.
        // - Detect boxing inside methods marked with a [HotPath] attribute (to be defined) or known performance-critical contexts.
    }
}

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LargeStructureAnalyzer : DiagnosticAnalyzer
{
    public const string LargeStructInHotPathId = "VCPERF004";

    private static readonly DiagnosticDescriptor LargeStructInHotPathRule = new(
        id: LargeStructInHotPathId,
        title: "Avoid large structs in hot paths",
        messageFormat: "Consider avoiding use of large struct '{0}' in performance-critical code.",
        category: "Performance",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(LargeStructInHotPathRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();    
    }
}