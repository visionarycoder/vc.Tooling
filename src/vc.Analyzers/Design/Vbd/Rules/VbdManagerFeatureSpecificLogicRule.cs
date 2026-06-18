using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Design.Vbd.Rules;

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
        context.RegisterSymbolAction(action: AnalyzeMethod, symbolKinds: SymbolKind.Method);
    }

    private static void AnalyzeMethod(SymbolAnalysisContext context)
    {
        if (context.Symbol is not IMethodSymbol method || method.MethodKind != MethodKind.Ordinary)
        {
            return;
        }

        if (!method.ContainingType.Name.EndsWith(value: "Manager"))
        {
            return;
        }

        var name = method.Name;
        if (name.Contains(value: "Feature") || name.Contains(value: "Experiment") || name.EndsWith(value: "V1") || name.EndsWith(value: "V2"))
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: method.Locations.FirstOrDefault(), messageArgs: method.Name));
        }
    }
}

