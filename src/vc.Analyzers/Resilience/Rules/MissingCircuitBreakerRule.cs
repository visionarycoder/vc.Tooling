using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Resilience.Rules;

internal sealed class MissingCircuitBreakerRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ResilienceCircuitBreakerMissing,
        title: "Missing circuit breaker",
        messageFormat: "External call '{0}' is not wrapped in a circuit breaker policy.",
        category: "Resilience",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(action: AnalyzeInvocation, syntaxKinds: SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        var symbol = context.SemanticModel.GetSymbolInfo(expression: invocation, cancellationToken: context.CancellationToken).Symbol as IMethodSymbol;
        if (symbol is null || !IsExternalCall(methodSymbol: symbol) || IsInsideCircuitBreaker(node: invocation))
        {
            return;
        }

        context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: invocation.GetLocation(), messageArgs: symbol.Name));
    }

    private static bool IsExternalCall(IMethodSymbol methodSymbol)
    {
        var ns = methodSymbol.ContainingNamespace?.ToDisplayString();
        return ns is not null &&
               (ns.StartsWith(value: "System.Net.Http", comparisonType: System.StringComparison.Ordinal) ||
                ns.Contains(value: "SqlClient", comparisonType: System.StringComparison.Ordinal) ||
                ns.Contains(value: "Redis", comparisonType: System.StringComparison.Ordinal) ||
                ns.Contains(value: "Mongo", comparisonType: System.StringComparison.Ordinal) ||
                ns.Contains(value: "Cosmos", comparisonType: System.StringComparison.Ordinal));
    }

    private static bool IsInsideCircuitBreaker(SyntaxNode node)
    {
        var current = node.Parent;
        while (current is not null)
        {
            if (current is InvocationExpressionSyntax invocation && invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var name = memberAccess.Name.Identifier.Text;
                if (name.Contains(value: "ExecuteAsync", comparisonType: System.StringComparison.Ordinal) ||
                    name.Contains(value: "Execute", comparisonType: System.StringComparison.Ordinal) ||
                    name.Contains(value: "WrapAsync", comparisonType: System.StringComparison.Ordinal) ||
                    name.Contains(value: "Wrap", comparisonType: System.StringComparison.Ordinal))
                {
                    return true;
                }
            }

            current = current.Parent;
        }

        return false;
    }
}