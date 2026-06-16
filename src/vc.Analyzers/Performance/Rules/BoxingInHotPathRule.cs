using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Performance.Rules;

internal sealed class BoxingInHotPathRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.PerfBoxing,
        "Avoid boxing in hot paths",
        "Consider avoiding boxing of '{0}' in performance-critical code.",
        "Performance",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
    }
}