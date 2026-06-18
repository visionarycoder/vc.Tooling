using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Design.Vbd.Rules;

internal sealed class VbdAccessBoundaryViolationRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.VbdAccessVaultBoundaryViolation,
        title: "Access boundary violation",
        messageFormat: "Access component '{0}' depends on manager/engine type '{1}'.",
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

        var badDependency = type.GetMembers().OfType<IFieldSymbol>()
            .Select(selector: f => f.Type.Name)
            .Concat(second: type.GetMembers().OfType<IPropertySymbol>().Select(selector: p => p.Type.Name))
            .FirstOrDefault(predicate: n => n.EndsWith(value: "Manager") || n.EndsWith(value: "Engine"));

        if (badDependency != null)
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: type.Locations.FirstOrDefault(), messageArgs: [type.Name, badDependency]));
        }
    }
}

