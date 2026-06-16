using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Vbd.Rules;

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
        context.RegisterSymbolAction(AnalyzeField, SymbolKind.Field);
    }

    private static void AnalyzeField(SymbolAnalysisContext context)
    {
        if (context.Symbol is not IFieldSymbol field)
        {
            return;
        }

        var containingType = field.ContainingType;
        if (containingType == null || !containingType.Name.EndsWith("Engine"))
        {
            return;
        }

        if (!field.IsReadOnly && !field.IsConst)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, field.Locations.FirstOrDefault(), containingType.Name, field.Name));
        }
    }
}

