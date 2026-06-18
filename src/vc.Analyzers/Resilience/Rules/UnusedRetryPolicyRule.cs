using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Resilience.Rules;

internal sealed class UnusedRetryPolicyRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ResilienceRetryPolicyExcessive,
        title: "Retry policy declared but never used",
        messageFormat: "Retry policy '{0}' is declared but never applied.",
        category: "Resilience",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(action: AnalyzePolicyDeclaration, symbolKinds: new[]{SymbolKind.Field, SymbolKind.Property});
    }

    private static void AnalyzePolicyDeclaration(SymbolAnalysisContext context)
    {
        var symbol = context.Symbol;
        if (!IsRetryPolicy(symbol: symbol) || IsUsed(symbol: symbol))
        {
            return;
        }

        context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: symbol.Locations.FirstOrDefault(), messageArgs: symbol.Name));
    }

    private static bool IsRetryPolicy(ISymbol symbol)
    {
        var type = symbol switch
        {
            IFieldSymbol field => field.Type,
            IPropertySymbol property => property.Type,
            _ => null
        };

        return type is not null && (type.Name.Contains(value: "Retry", comparisonType: System.StringComparison.Ordinal) || type.Name.Contains(value: "AsyncPolicy", comparisonType: System.StringComparison.Ordinal) || type.Name.Contains(value: "Policy", comparisonType: System.StringComparison.Ordinal));
    }

    private static bool IsUsed(ISymbol symbol)
    {
        foreach (var reference in symbol.DeclaringSyntaxReferences)
        {
            foreach (var invocation in reference.GetSyntax().SyntaxTree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess && memberAccess.Expression is IdentifierNameSyntax identifier && identifier.Identifier.Text == symbol.Name)
                {
                    return true;
                }
            }
        }

        return false;
    }
}