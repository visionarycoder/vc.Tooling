using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Distributed.Rules;

internal sealed class MissingAsyncRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.DistributedRepositoryAsyncMissing,
        title: "Repository methods should be async",
        messageFormat: "Repository method '{0}' should be asynchronous.",
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

            if (method.ReturnType is INamedTypeSymbol returnType && (returnType.Name == "Task" || returnType.Name == "ValueTask"))
            {
                continue;
            }

            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: method.Locations.FirstOrDefault(), messageArgs: method.Name));
        }
    }

    private static bool LooksLikeRepository(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.Name.EndsWith(value: "Repository", comparisonType: System.StringComparison.Ordinal) ||
               typeSymbol.Interfaces.Any(predicate: interfaceSymbol => interfaceSymbol.Name == "IRepository");
    }
}