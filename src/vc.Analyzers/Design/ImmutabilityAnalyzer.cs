namespace Vc.Analyzers.Design;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ImmutabilityAnalyzer : DiagnosticAnalyzer
{
    public const string MutableRecordFieldId = "VCDESIGN001";

    private static readonly DiagnosticDescriptor MutableRecordFieldRule = new(
        id: MutableRecordFieldId,
        title: "Record should be immutable",
        messageFormat: "Record '{0}' contains mutable members; prefer init-only or readonly members.",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MutableRecordFieldRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TODO:
        // - Register symbol action for record types.
        // - Inspect fields and properties for mutability (setters, mutable collections).
    }
}
