namespace Vc.Analyzers.Security;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SecurityUsageAnalyzer : DiagnosticAnalyzer
{
    public const string MissingSecureAttributeId = "VCSEC001";

    private static readonly DiagnosticDescriptor MissingSecureAttributeRule = new(
        id: MissingSecureAttributeId,
        title: "Public API should be secured",
        messageFormat: "Public method '{0}' should be annotated with [Secure] or equivalent security attribute.",
        category: "Security",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MissingSecureAttributeRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TODO:
        // - Register symbol action for public methods in API assemblies.
        // - Detect presence of [Secure] or other security attributes.
        // - Report diagnostics when public endpoints are not secured.
    }
}
