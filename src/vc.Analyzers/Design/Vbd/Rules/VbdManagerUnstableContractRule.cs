using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Design.Vbd.Rules;

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

        var returnName = method.ReturnType.Name;
        if (returnName is "Object" or "Dynamic" || returnName.EndsWith(value: "Dto") || returnName.EndsWith(value: "Model"))
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: method.Locations.FirstOrDefault(), messageArgs: [method.Name, returnName]));
        }
    }
}

