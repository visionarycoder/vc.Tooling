using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Vbd.Rules;

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
        context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
    }

    private static void AnalyzeType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol type || type.TypeKind != TypeKind.Class)
        {
            return;
        }

        var badDependency = type.GetMembers().OfType<IFieldSymbol>()
            .Select(f => f.Type.Name)
            .Concat(type.GetMembers().OfType<IPropertySymbol>().Select(p => p.Type.Name))
            .FirstOrDefault(n => n.EndsWith("Manager") || n.EndsWith("Engine"));

        if (badDependency != null)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, type.Locations.FirstOrDefault(), type.Name, badDependency));
        }
    }
}

