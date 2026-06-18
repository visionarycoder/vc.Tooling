using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Distributed.Rules;

internal sealed class DomainReturnTypeRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.DistributedRepositoryContractViolation,
        title: "Repository should not return domain entities directly",
        messageFormat: "Repository method '{0}' returns domain type '{1}'. Repositories should return aggregates or DTOs.",
        category: "Distributed",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(action: AnalyzeRepository, symbolKinds: SymbolKind.NamedType);
    }

    private static void AnalyzeRepository(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol typeSymbol || !LooksLikeRepository(typeSymbol: typeSymbol))
        {
            return;
        }

        foreach (var method in typeSymbol.GetMembers().OfType<IMethodSymbol>())
        {
            if (method.MethodKind != MethodKind.Ordinary)
            {
                continue;
            }

            if (method.ReturnType is not INamedTypeSymbol returnType)
            {
                continue;
            }

            if (returnType.Name.EndsWith(value: "Entity", comparisonType: System.StringComparison.Ordinal) ||
                returnType.Name.EndsWith(value: "Model", comparisonType: System.StringComparison.Ordinal) ||
                returnType.Name.EndsWith(value: "Domain", comparisonType: System.StringComparison.Ordinal))
            {
                context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: method.Locations.FirstOrDefault(), messageArgs: [method.Name, returnType.Name]));
            }
        }
    }

    private static bool LooksLikeRepository(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.Name.EndsWith(value: "Repository", comparisonType: System.StringComparison.Ordinal) ||
               typeSymbol.Interfaces.Any(predicate: interfaceSymbol => interfaceSymbol.Name == "IRepository");
    }
}