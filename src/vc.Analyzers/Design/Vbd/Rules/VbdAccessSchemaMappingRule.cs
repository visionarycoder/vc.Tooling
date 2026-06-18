using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Design.Vbd.Rules;

internal sealed class VbdAccessSchemaMappingRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.VbdAccessVaultSchemaMappingMissing,
        title: "Schema mapping missing",
        messageFormat: "Access component '{0}' returns persistence models but exposes no map/transform method.",
        category: "VbdAccess",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(action: AnalyzeType, symbolKinds: SymbolKind.NamedType);
    }

    private static void AnalyzeType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol type || type.TypeKind != TypeKind.Class)
        {
            return;
        }

        var returnsEntity = type.GetMembers().OfType<IMethodSymbol>()
            .Any(predicate: m => m.MethodKind == MethodKind.Ordinary && m.ReturnType.Name.EndsWith(value: "Entity"));

        if (!returnsEntity)
        {
            return;
        }

        var hasMappingMethod = type.GetMembers().OfType<IMethodSymbol>()
            .Any(predicate: m => m.MethodKind == MethodKind.Ordinary &&
                                 (m.Name.StartsWith(value: "Map") || m.Name.StartsWith(value: "Transform") || m.Name.StartsWith(value: "ToDomain")));

        if (!hasMappingMethod)
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: type.Locations.FirstOrDefault(), messageArgs: type.Name));
        }
    }
}

