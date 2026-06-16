using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Rules;

internal sealed class MutableRecordFieldRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        "VCDESIGN001",
        "Record should be immutable",
        "Record '{0}' contains mutable members; prefer init-only or readonly members.",
        "Design",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
    }
}