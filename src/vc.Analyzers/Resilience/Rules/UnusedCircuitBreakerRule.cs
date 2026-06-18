using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Resilience.Rules;

internal sealed class UnusedCircuitBreakerRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ResilienceCircuitBreakerUnused,
        title: "Circuit breaker declared but never used",
        messageFormat: "Circuit breaker policy '{0}' is declared but never applied.",
        category: "Resilience",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(action: AnalyzeFieldOrProperty, symbolKinds: new[]{SymbolKind.Field, SymbolKind.Property});
    }

    private static void AnalyzeFieldOrProperty(SymbolAnalysisContext context)
    {
        var symbol = context.Symbol;
        if (!IsCircuitBreakerPolicy(symbol: symbol) || IsUsed(symbol: symbol))
        {
            return;
        }

        context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: symbol.Locations.FirstOrDefault(), messageArgs: symbol.Name));
    }

    private static bool IsCircuitBreakerPolicy(ISymbol symbol)
    {
        var type = symbol switch
        {
            IFieldSymbol field => field.Type,
            IPropertySymbol property => property.Type,
            _ => null
        };

        return type is not null && (type.Name.Contains(value: "CircuitBreaker", comparisonType: System.StringComparison.Ordinal) || type.Name.Contains(value: "AsyncPolicy", comparisonType: System.StringComparison.Ordinal) || type.Name.Contains(value: "Policy", comparisonType: System.StringComparison.Ordinal));
    }

    private static bool IsUsed(ISymbol symbol)
    {
        foreach (var reference in symbol.DeclaringSyntaxReferences)
        {
            var root = reference.GetSyntax().SyntaxTree.GetRoot();
            foreach (var invocation in root.DescendantNodes().OfType<InvocationExpressionSyntax>())
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