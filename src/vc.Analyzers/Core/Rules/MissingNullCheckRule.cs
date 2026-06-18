using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Core.Rules;

internal sealed class MissingNullCheckRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.NullSafetyMissingCheck,
        title: "Parameter should be null-checked",
        messageFormat: "Parameter '{0}' should be validated for null.",
        category: "NullSafety",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
    }
}