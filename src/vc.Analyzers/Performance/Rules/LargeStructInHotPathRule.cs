using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Performance.Rules;

internal sealed class LargeStructInHotPathRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: "VCPERF004",
        title: "Avoid large structs in hot paths",
        messageFormat: "Consider avoiding use of large struct '{0}' in performance-critical code.",
        category: "Performance",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
    }
}