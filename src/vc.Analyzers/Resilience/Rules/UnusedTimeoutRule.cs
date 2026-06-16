using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Resilience.Rules;

internal sealed class UnusedTimeoutRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.ResilienceTimeoutExcessive,
        "Timeout declared but never used",
        "Timeout value '{0}' is declared but never applied to any external call.",
        "Resilience",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyzeTimeoutDeclaration, SymbolKind.Field, SymbolKind.Property);
    }

    private static void AnalyzeTimeoutDeclaration(SymbolAnalysisContext context)
    {
        if (!IsTimeoutValue(context.Symbol) || IsUsed(context.Symbol))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(descriptor, context.Symbol.Locations.FirstOrDefault(), context.Symbol.Name));
    }

    private static bool IsTimeoutValue(ISymbol symbol)
    {
        var type = symbol switch
        {
            IFieldSymbol field => field.Type,
            IPropertySymbol property => property.Type,
            _ => null
        };

        return type is not null && (type.Name.Contains("TimeSpan", System.StringComparison.Ordinal) || symbol.Name.Contains("Timeout", System.StringComparison.Ordinal) || symbol.Name.Contains("Delay", System.StringComparison.Ordinal));
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