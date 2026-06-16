using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Resilience.Rules;

internal sealed class UnusedCircuitBreakerRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.ResilienceCircuitBreakerUnused,
        "Circuit breaker declared but never used",
        "Circuit breaker policy '{0}' is declared but never applied.",
        "Resilience",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyzeFieldOrProperty, SymbolKind.Field, SymbolKind.Property);
    }

    private static void AnalyzeFieldOrProperty(SymbolAnalysisContext context)
    {
        var symbol = context.Symbol;
        if (!IsCircuitBreakerPolicy(symbol) || IsUsed(symbol))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(descriptor, symbol.Locations.FirstOrDefault(), symbol.Name));
    }

    private static bool IsCircuitBreakerPolicy(ISymbol symbol)
    {
        var type = symbol switch
        {
            IFieldSymbol field => field.Type,
            IPropertySymbol property => property.Type,
            _ => null
        };

        return type is not null && (type.Name.Contains("CircuitBreaker", System.StringComparison.Ordinal) || type.Name.Contains("AsyncPolicy", System.StringComparison.Ordinal) || type.Name.Contains("Policy", System.StringComparison.Ordinal));
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