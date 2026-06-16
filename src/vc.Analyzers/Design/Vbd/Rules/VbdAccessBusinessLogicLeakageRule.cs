using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Vbd.Rules;

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
        context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
    }

    private static void AnalyzeType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol type || type.TypeKind != TypeKind.Class)
        {
            return;
        }

        if (!LooksLikeAccessComponent(type.Name))
        {
            return;
        }

        var suspiciousMethod = type.GetMembers().OfType<IMethodSymbol>()
            .FirstOrDefault(m => m.MethodKind == MethodKind.Ordinary &&
                                 (m.Name.StartsWith("Calculate") || m.Name.StartsWith("Compute") || m.Name.StartsWith("Decide")));

        if (suspiciousMethod != null)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, suspiciousMethod.Locations.FirstOrDefault(), type.Name, suspiciousMethod.Name));
        }
    }

    private static bool LooksLikeAccessComponent(string typeName)
    {
        return typeName.EndsWith("Repository") || typeName.EndsWith("Store") || typeName.EndsWith("Gateway") || typeName.EndsWith("Accessor");
    }
}

