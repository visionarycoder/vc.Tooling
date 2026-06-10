namespace Vc.Analyzers.Core;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NullSafetyAnalyzer : DiagnosticAnalyzer
{
    public const string MissingNullCheckId = "VCNULL001";

    private static readonly DiagnosticDescriptor MissingNullCheckRule = new(
        id: MissingNullCheckId,
        title: "Parameter should be null-checked",
        messageFormat: "Parameter '{0}' should be validated for null.",
        category: "NullSafety",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MissingNullCheckRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TODO: Register symbol action for methods:
        // - For reference-type parameters, check if there is a null-check or ArgumentNullException.ThrowIfNull.
        // - Report diagnostics for parameters that are used without prior null validation.
    }
}
