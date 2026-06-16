using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Vbd.Rules;

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
        context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
    }

    private static void AnalyzeType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol type || type.TypeKind != TypeKind.Class)
        {
            return;
        }

        if (!type.Name.EndsWith("Engine"))
        {
            return;
        }

        var infraRef = type.GetMembers().OfType<IFieldSymbol>()
            .Select(f => f.Type.ContainingNamespace?.ToDisplayString() ?? string.Empty)
            .Concat(type.GetMembers().OfType<IPropertySymbol>().Select(p => p.Type.ContainingNamespace?.ToDisplayString() ?? string.Empty))
            .FirstOrDefault(ns => ns.Contains("System.Net.Http") || ns.Contains("EntityFramework") || ns.Contains("SqlClient"));

        if (!string.IsNullOrWhiteSpace(infraRef))
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, type.Locations.FirstOrDefault(), type.Name, infraRef));
        }
    }
}

