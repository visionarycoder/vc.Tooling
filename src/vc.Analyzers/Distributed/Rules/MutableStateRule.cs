using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Distributed.Rules;

internal sealed class MutableStateRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.DistributedEventSourcingMutableState,
        title: "Event-sourced aggregate must not mutate state outside Apply()",
        messageFormat: "State member '{0}' is mutated outside an Apply() method.",
        category: "Distributed",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(action: AnalyzeAggregate, symbolKinds: SymbolKind.NamedType);
    }

    private static void AnalyzeAggregate(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol typeSymbol || !IsAggregate(typeSymbol: typeSymbol))
        {
            return;
        }

        var applyMethods = typeSymbol.GetMembers().OfType<IMethodSymbol>()
            .Where(predicate: method => method.Name == "Apply" && method.Parameters.Length == 1)
            .ToList();

        var applySyntaxes = applyMethods
            .Where(predicate: method => method.DeclaringSyntaxReferences.Length > 0)
            .Select(selector: method => method.DeclaringSyntaxReferences[index: 0].GetSyntax())
            .OfType<MethodDeclarationSyntax>()
            .ToList();

        foreach (var method in typeSymbol.GetMembers().OfType<IMethodSymbol>())
        {
            if (method.DeclaringSyntaxReferences.Length == 0)
            {
                continue;
            }

            var syntax = method.DeclaringSyntaxReferences[index: 0].GetSyntax() as MethodDeclarationSyntax;
            if (syntax is null || applySyntaxes.Contains(item: syntax))
            {
                continue;
            }

            var model = context.Compilation.GetSemanticModel(syntaxTree: syntax.SyntaxTree);
            foreach (var assignment in syntax.DescendantNodes().OfType<AssignmentExpressionSyntax>())
            {
                var symbol = model.GetSymbolInfo(expression: assignment.Left).Symbol;
                if (symbol is IFieldSymbol or IPropertySymbol)
                {
                    context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: assignment.GetLocation(), messageArgs: symbol.Name));
                }
            }
        }
    }

    private static bool IsAggregate(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.Name.EndsWith(value: "Aggregate", comparisonType: System.StringComparison.Ordinal) ||
               typeSymbol.Interfaces.Any(predicate: interfaceSymbol => interfaceSymbol.Name == "IAggregate");
    }
}