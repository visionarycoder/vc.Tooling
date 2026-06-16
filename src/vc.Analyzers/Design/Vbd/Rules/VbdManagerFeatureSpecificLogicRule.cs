using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Vbd.Rules;

internal sealed class VbdManagerFeatureSpecificLogicRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.VbdManagerVaultFeatureSpecificLogic,
        title: "Manager contains feature-specific logic",
        messageFormat: "Manager method '{0}' appears feature-specific; extract to policy/strategy.",
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

        var name = method.Name;
        if (name.Contains("Feature") || name.Contains("Experiment") || name.EndsWith("V1") || name.EndsWith("V2"))
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, method.Locations.FirstOrDefault(), method.Name));
        }
    }
}

