using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Distributed.Rules;

internal sealed class MutableStateRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.DistributedEventSourcingMutableState,
        "Event-sourced aggregate must not mutate state outside Apply()",
        "State member '{0}' is mutated outside an Apply() method.",
        "Distributed",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyzeAggregate, SymbolKind.NamedType);
    }

    private static void AnalyzeAggregate(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol typeSymbol || !IsAggregate(typeSymbol))
        {
            return;
        }

        var applyMethods = typeSymbol.GetMembers().OfType<IMethodSymbol>()
            .Where(method => method.Name == "Apply" && method.Parameters.Length == 1)
            .ToList();

        var applySyntaxes = applyMethods
            .Where(method => method.DeclaringSyntaxReferences.Length > 0)
            .Select(method => method.DeclaringSyntaxReferences[0].GetSyntax())
            .OfType<MethodDeclarationSyntax>()
            .ToList();

        foreach (var method in typeSymbol.GetMembers().OfType<IMethodSymbol>())
        {
            if (method.DeclaringSyntaxReferences.Length == 0)
            {
                continue;
            }

            var syntax = method.DeclaringSyntaxReferences[0].GetSyntax() as MethodDeclarationSyntax;
            if (syntax is null || applySyntaxes.Contains(syntax))
            {
                continue;
            }

            var model = context.Compilation.GetSemanticModel(syntax.SyntaxTree);
            foreach (var assignment in syntax.DescendantNodes().OfType<AssignmentExpressionSyntax>())
            {
                var symbol = model.GetSymbolInfo(assignment.Left).Symbol;
                if (symbol is IFieldSymbol or IPropertySymbol)
                {
                    context.ReportDiagnostic(Diagnostic.Create(descriptor, assignment.GetLocation(), symbol.Name));
                }
            }
        }
    }

    private static bool IsAggregate(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.Name.EndsWith("Aggregate", System.StringComparison.Ordinal) ||
               typeSymbol.Interfaces.Any(interfaceSymbol => interfaceSymbol.Name == "IAggregate");
    }
}