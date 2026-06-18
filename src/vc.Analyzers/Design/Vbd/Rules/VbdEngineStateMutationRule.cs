using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Design.Vbd.Rules;

internal sealed class VbdEngineStateMutationRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.VbdEngineVaultStateViolation,
        title: "Engine has mutable state",
        messageFormat: "Engine type '{0}' contains mutable field '{1}'.",
        category: "VbdEngine",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(action: AnalyzeField, symbolKinds: SymbolKind.Field);
    }

    private static void AnalyzeField(SymbolAnalysisContext context)
    {
        if (context.Symbol is not IFieldSymbol field)
        {
            return;
        }

        var containingType = field.ContainingType;
        if (containingType == null || !containingType.Name.EndsWith(value: "Engine"))
        {
            return;
        }

        if (!field.IsReadOnly && !field.IsConst)
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: field.Locations.FirstOrDefault(), messageArgs: [containingType.Name, field.Name]));
        }
    }
}

