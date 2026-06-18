using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Distributed.Rules;

internal sealed class MissingApplyMethodRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.DistributedEventSourcingMissingApply,
        title: "Missing Apply method for event",
        messageFormat: "Aggregate '{0}' does not define an Apply({1}) method for event '{1}'.",
        category: "Distributed",
        defaultSeverity: DiagnosticSeverity.Warning,
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

        var eventsRaised = FindRaisedEvents(compilation: context.Compilation, typeSymbol: typeSymbol);
        var eventsApplied = new HashSet<ITypeSymbol>(collection: applyMethods.Select(selector: method => method.Parameters[index: 0].Type), comparer: SymbolEqualityComparer.Default);

        foreach (var eventType in eventsRaised)
        {
            if (!eventsApplied.Contains(item: eventType))
            {
                context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: typeSymbol.Locations.FirstOrDefault(), messageArgs: [typeSymbol.Name, eventType.Name]));
            }
        }
    }

    private static bool IsAggregate(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.Name.EndsWith(value: "Aggregate", comparisonType: System.StringComparison.Ordinal) ||
               typeSymbol.Interfaces.Any(predicate: interfaceSymbol => interfaceSymbol.Name == "IAggregate");
    }

    private static ImmutableHashSet<INamedTypeSymbol> FindRaisedEvents(Compilation compilation, INamedTypeSymbol typeSymbol)
    {
        var builder = ImmutableHashSet.CreateBuilder<INamedTypeSymbol>(equalityComparer: SymbolEqualityComparer.Default);

        foreach (var method in typeSymbol.GetMembers().OfType<IMethodSymbol>())
        {
            if (method.DeclaringSyntaxReferences.Length == 0)
            {
                continue;
            }

            var syntax = method.DeclaringSyntaxReferences[index: 0].GetSyntax() as MethodDeclarationSyntax;
            if (syntax is null)
            {
                continue;
            }

            var model = compilation.GetSemanticModel(syntaxTree: syntax.SyntaxTree);
            foreach (var creation in syntax.DescendantNodes().OfType<ObjectCreationExpressionSyntax>())
            {
                if (model.GetTypeInfo(expression: creation).Type is INamedTypeSymbol eventType && eventType.Name.EndsWith(value: "Event", comparisonType: System.StringComparison.Ordinal))
                {
                    builder.Add(item: eventType);
                }
            }
        }

        return builder.ToImmutable();
    }
}