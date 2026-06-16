using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Performance.Rules;

internal sealed class LargeStructInHotPathRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        "VCPERF004",
        "Avoid large structs in hot paths",
        "Consider avoiding use of large struct '{0}' in performance-critical code.",
        "Performance",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
    }
}