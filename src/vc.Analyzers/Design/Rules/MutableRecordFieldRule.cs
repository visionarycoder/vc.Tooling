using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Design.Rules;

internal sealed class MutableRecordFieldRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: "VCDESIGN001",
        title: "Record should be immutable",
        messageFormat: "Record '{0}' contains mutable members; prefer init-only or readonly members.",
        category: "Design",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
    }
}