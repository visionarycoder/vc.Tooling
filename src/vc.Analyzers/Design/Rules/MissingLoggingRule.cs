using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Design.Rules;

internal sealed class MissingLoggingRule : IAnalyzerRule
{
    private static readonly string[] LoggingMethodHints = ["Log", "Trace", "Error", "Warn", "Critical", "TrackException"
    ];

    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: "VCDESIGN002",
        title: "Missing logging in catch block",
        messageFormat: "Exception caught but not logged. Catch blocks should log, wrap, or rethrow exceptions.",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(action: AnalyzeCatchClause, syntaxKinds: SyntaxKind.CatchClause);
    }

    private static void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not CatchClauseSyntax catchClause || catchClause.Block is null)
        {
            return;
        }

        if (ContainsRethrow(block: catchClause.Block) || ContainsLogging(semanticModel: context.SemanticModel, block: catchClause.Block, cancellationToken: context.CancellationToken))
        {
            return;
        }

        context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: catchClause.CatchKeyword.GetLocation()));
    }

    private static bool ContainsRethrow(BlockSyntax block)
    {
        return block.DescendantNodes().OfType<ThrowStatementSyntax>().Any(predicate: throwStatement => throwStatement.Expression is null);
    }

    private static bool ContainsLogging(SemanticModel semanticModel, BlockSyntax block, System.Threading.CancellationToken cancellationToken)
    {
        foreach (var invocation in block.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var symbol = semanticModel.GetSymbolInfo(expression: invocation, cancellationToken: cancellationToken).Symbol as IMethodSymbol;
            if (symbol is null)
            {
                continue;
            }

            if (LoggingMethodHints.Any(predicate: hint => symbol.Name.Contains(value: hint, comparisonType: StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            var containingType = symbol.ContainingType?.Name;
            if (containingType is not null && (containingType.Contains(value: "Logger", comparisonType: StringComparison.OrdinalIgnoreCase) || containingType.Contains(value: "Telemetry", comparisonType: StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
        }

        return false;
    }
}