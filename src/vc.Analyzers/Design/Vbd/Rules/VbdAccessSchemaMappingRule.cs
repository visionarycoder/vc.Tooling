using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Vbd.Rules;

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
        context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
    }

    private static void AnalyzeType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol type || type.TypeKind != TypeKind.Class)
        {
            return;
        }

        var returnsEntity = type.GetMembers().OfType<IMethodSymbol>()
            .Any(m => m.MethodKind == MethodKind.Ordinary && m.ReturnType.Name.EndsWith("Entity"));

        if (!returnsEntity)
        {
            return;
        }

        var hasMappingMethod = type.GetMembers().OfType<IMethodSymbol>()
            .Any(m => m.MethodKind == MethodKind.Ordinary &&
                      (m.Name.StartsWith("Map") || m.Name.StartsWith("Transform") || m.Name.StartsWith("ToDomain")));

        if (!hasMappingMethod)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, type.Locations.FirstOrDefault(), type.Name));
        }
    }
}

