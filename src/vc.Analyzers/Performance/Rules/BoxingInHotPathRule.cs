using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Performance.Rules;

internal sealed class BoxingInHotPathRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.PerfBoxing,
        title: "Avoid boxing in hot paths",
        messageFormat: "Consider avoiding boxing of '{0}' in performance-critical code.",
        category: "Performance",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
    }
}