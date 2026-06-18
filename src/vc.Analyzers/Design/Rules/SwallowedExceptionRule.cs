using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Design.Rules;

internal sealed class SwallowedExceptionRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.DesignExceptionSafetySwallowed,
        title: "Exception is swallowed",
        messageFormat: "Exception is caught but not logged, wrapped, or rethrown.",
        category: "ExceptionSafety",
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

        if (!ContainsThrow(block: catchClause.Block) && !ContainsLogging(semanticModel: context.SemanticModel, block: catchClause.Block, cancellationToken: context.CancellationToken))
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: catchClause.CatchKeyword.GetLocation()));
        }
    }

    private static bool ContainsThrow(BlockSyntax block)
    {
        return block.DescendantNodes().OfType<ThrowStatementSyntax>().Any();
    }

    private static bool ContainsLogging(SemanticModel semanticModel, BlockSyntax block, System.Threading.CancellationToken cancellationToken)
    {
        foreach (var invocation in block.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var symbol = semanticModel.GetSymbolInfo(expression: invocation, cancellationToken: cancellationToken).Symbol as IMethodSymbol;
            if (symbol is not null)
            {
                var name = symbol.Name;
                if (name.Contains(value: "Log", comparisonType: StringComparison.OrdinalIgnoreCase) || name.Contains(value: "Trace", comparisonType: StringComparison.OrdinalIgnoreCase) || name.Contains(value: "Error", comparisonType: StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }
        }

        return false;
    }
}