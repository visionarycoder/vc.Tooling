namespace vc.Analyzers.Design.Architecure.Vc.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LayeringAnalyzer : DiagnosticAnalyzer
{
    public const string LayerViolationId = "VCARCH001";

    private static readonly DiagnosticDescriptor LayerViolationRule = new(
        id: LayerViolationId,
        title: "Layering rule violation",
        messageFormat: "Type '{0}' references '{1}', which violates the configured layering rules.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(LayerViolationRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TODO:
        // - Read layering configuration (e.g., via additional files or attributes).
        // - Register symbol or compilation actions to inspect namespace and project dependencies.
    }
}