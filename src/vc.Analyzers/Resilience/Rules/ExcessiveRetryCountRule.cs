using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Resilience.Rules;

internal sealed class ExcessiveRetryCountRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ResilienceRetryConfigurationIssue,
        title: "Retry count too high",
        messageFormat: "Retry policy '{0}' retries {1} times, which exceeds recommended limits.",
        category: "Resilience",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(action: AnalyzePolicyDeclaration, symbolKinds: new[]{SymbolKind.Field, SymbolKind.Property});
    }

    private static void AnalyzePolicyDeclaration(SymbolAnalysisContext context)
    {
        var retryCount = GetRetryCount(symbol: context.Symbol);
        if (retryCount > 5)
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: context.Symbol.Locations.FirstOrDefault(), messageArgs: [context.Symbol.Name, retryCount]));
        }
    }

    private static int GetRetryCount(ISymbol symbol)
    {
        foreach (var reference in symbol.DeclaringSyntaxReferences)
        {
            foreach (var invocation in reference.GetSyntax().SyntaxTree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    var name = memberAccess.Name.Identifier.Text;
                    if ((name.Contains(value: "Retry", comparisonType: System.StringComparison.Ordinal) || name.Contains(value: "WaitAndRetry", comparisonType: System.StringComparison.Ordinal)) &&
                        invocation.ArgumentList.Arguments.Count > 0 &&
                        invocation.ArgumentList.Arguments[index: 0].Expression is LiteralExpressionSyntax literal &&
                        literal.Token.Value is int retryCount)
                    {
                        return retryCount;
                    }
                }
            }
        }

        return 0;
    }
}