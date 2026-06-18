using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Design.Vbd.Rules;

internal sealed class VbdEngineInfrastructureAccessRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.VbdEngineVaultInfrastructureAccess,
        title: "Engine accesses infrastructure",
        messageFormat: "Engine component '{0}' references infrastructure namespace '{1}'.",
        category: "VbdEngine",
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

        if (!type.Name.EndsWith(value: "Engine"))
        {
            return;
        }

        var infraRef = type.GetMembers().OfType<IFieldSymbol>()
            .Select(selector: f => f.Type.ContainingNamespace?.ToDisplayString() ?? string.Empty)
            .Concat(second: type.GetMembers().OfType<IPropertySymbol>().Select(selector: p => p.Type.ContainingNamespace?.ToDisplayString() ?? string.Empty))
            .FirstOrDefault(predicate: ns => ns.Contains(value: "System.Net.Http") || ns.Contains(value: "EntityFramework") || ns.Contains(value: "SqlClient"));

        if (!string.IsNullOrWhiteSpace(value: infraRef))
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: type.Locations.FirstOrDefault(), messageArgs: [type.Name, infraRef]));
        }
    }
}

