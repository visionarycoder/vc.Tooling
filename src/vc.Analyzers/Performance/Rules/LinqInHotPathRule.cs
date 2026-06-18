using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Performance.Rules;

internal sealed class LinqInHotPathRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.PerfLinqInHotPath,
        title: "Avoid LINQ in hot paths",
        messageFormat: "Consider avoiding LINQ call '{0}' in performance-critical code.",
        category: "Performance",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
    }
}