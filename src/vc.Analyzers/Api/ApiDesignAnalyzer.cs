namespace Vc.Analyzers.Api;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ApiDesignAnalyzer : DiagnosticAnalyzer
{
    public const string MissingApiControllerId = "VCAPI001";
    public const string NonRestfulNameId = "VCAPI002";

    private static readonly DiagnosticDescriptor MissingApiControllerRule = new(
        id: MissingApiControllerId,
        title: "API controller should be annotated",
        messageFormat: "Public controller '{0}' should be annotated with [ApiController].",
        category: "ApiDesign",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor NonRestfulNameRule = new(
        id: NonRestfulNameId,
        title: "Non-RESTful action name",
        messageFormat: "Action '{0}' may not follow RESTful naming conventions.",
        category: "ApiDesign",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MissingApiControllerRule, NonRestfulNameRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TODO:
        // - Register symbol action for controller types (classes inheriting from ControllerBase).
        // - Check for [ApiController] attribute.
        // - Inspect public action methods for naming patterns (e.g., Get*, Post*, Put*, Delete*).
    }
}

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ApiValidationAnalyzer : DiagnosticAnalyzer
{
    // ToDo: Define rules for validating API design, such as ensuring consistent use of validation attributes, checking for missing model validation, etc.
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => throw new NotImplementedException();

    public override void Initialize(AnalysisContext context)
    {
        throw new NotImplementedException();
    }
}

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ApiVersioningAnalyzer : DiagnosticAnalyzer
{
    // ToDo: Define rules for API versioning, such as ensuring controllers are properly versioned, checking for deprecated API usage, etc.
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => throw new NotImplementedException();

    public override void Initialize(AnalysisContext context)
    {
        throw new NotImplementedException();
    }
}

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ApiResponseTypeAnalyzer : DiagnosticAnalyzer
{
    // ToDo: Define rules for API response types, such as ensuring consistent use of IActionResult, checking for proper status code returns, etc.
    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => throw new NotImplementedException();

    public override void Initialize(AnalysisContext context)
    {
        throw new NotImplementedException();
    }
}