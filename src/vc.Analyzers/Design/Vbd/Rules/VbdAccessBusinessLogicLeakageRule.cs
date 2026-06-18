using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Design.Vbd.Rules;

internal sealed class VbdAccessBusinessLogicLeakageRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.VbdAccessVaultBusinessLogicLeakage,
        title: "Business logic leakage in access component",
        messageFormat: "Access component '{0}' appears to contain business logic method '{1}'.",
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

        if (!LooksLikeAccessComponent(typeName: type.Name))
        {
            return;
        }

        var suspiciousMethod = type.GetMembers().OfType<IMethodSymbol>()
            .FirstOrDefault(predicate: m => m.MethodKind == MethodKind.Ordinary &&
                                            (m.Name.StartsWith(value: "Calculate") || m.Name.StartsWith(value: "Compute") || m.Name.StartsWith(value: "Decide")));

        if (suspiciousMethod != null)
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: suspiciousMethod.Locations.FirstOrDefault(), messageArgs: [type.Name, suspiciousMethod.Name]));
        }
    }

    private static bool LooksLikeAccessComponent(string typeName)
    {
        return typeName.EndsWith(value: "Repository") || typeName.EndsWith(value: "Store") || typeName.EndsWith(value: "Gateway") || typeName.EndsWith(value: "Accessor");
    }
}

