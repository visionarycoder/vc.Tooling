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

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LoggingAnalyzer : DiagnosticAnalyzer
{
    public const string MissingLoggingId = "VCDESIGN002";

    private static readonly DiagnosticDescriptor MissingLoggingRule = new(
        MissingLoggingId,
        "Missing logging in catch block",
        "Exception caught but not logged. Catch blocks should log, wrap, or rethrow exceptions.",
        "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly string[] LoggingMethodHints =
    {
        "Log",
        "Trace",
        "Error",
        "Warn",
        "Critical",
        "TrackException"
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MissingLoggingRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeCatchClause, SyntaxKind.CatchClause);
    }

    private static void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not CatchClauseSyntax catchClause)
        {
            return;
        }

        var block = catchClause.Block;
        if (block is null)
        {
            return;
        }

        if (ContainsRethrow(block))
        {
            return;
        }

        if (ContainsLogging(context.SemanticModel, block, context.CancellationToken))
        {
            return;
        }

        context.ReportDiagnostic(
            Diagnostic.Create(
                MissingLoggingRule,
                catchClause.CatchKeyword.GetLocation()));
    }

    private static bool ContainsRethrow(BlockSyntax block)
    {
        foreach (var throwStatement in block.DescendantNodes().OfType<ThrowStatementSyntax>())
        {
            if (throwStatement.Expression is null)
            {
                return true;
            }
        }

        return false;
    }

    private static bool ContainsLogging(SemanticModel semanticModel, BlockSyntax block, System.Threading.CancellationToken cancellationToken)
    {
        foreach (var invocation in block.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var symbol = semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol as IMethodSymbol;
            if (symbol is null)
            {
                continue;
            }

            var methodName = symbol.Name;

            if (LoggingMethodHints.Any(hint =>
                methodName.Contains(hint, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            var containingType = symbol.ContainingType?.Name;
            if (containingType is null)
            {
                continue;
            }

            if (containingType.Contains("Logger", StringComparison.OrdinalIgnoreCase) ||
                containingType.Contains("Telemetry", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }
}
