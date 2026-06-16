using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Vbd.Rules;

internal sealed class VbdManagerUnstableContractRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.VbdManagerVaultUnstableContract,
        title: "Manager exposes unstable contract",
        messageFormat: "Manager method '{0}' uses unstable contract type '{1}'.",
        category: "VbdManager",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
    }

    private static void AnalyzeMethod(SymbolAnalysisContext context)
    {
        if (context.Symbol is not IMethodSymbol method || method.MethodKind != MethodKind.Ordinary)
        {
            return;
        }

        if (!method.ContainingType.Name.EndsWith("Manager"))
        {
            return;
        }

        var returnName = method.ReturnType.Name;
        if (returnName is "Object" or "Dynamic" || returnName.EndsWith("Dto") || returnName.EndsWith("Model"))
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, method.Locations.FirstOrDefault(), method.Name, returnName));
        }
    }
}

