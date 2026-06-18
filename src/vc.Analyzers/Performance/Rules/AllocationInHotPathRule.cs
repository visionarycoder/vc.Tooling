using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Performance.Rules;

internal sealed class AllocationInHotPathRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.PerfAllocationInHotPath,
        title: "Avoid allocations in hot paths",
        messageFormat: "Consider avoiding allocation of '{0}' in performance-critical code.",
        category: "Performance",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
    }
}