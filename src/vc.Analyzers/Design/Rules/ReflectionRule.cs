using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Design.Rules;

internal sealed class ReflectionRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.LegacyReflection,
        title: "Missing reflection configuration",
        messageFormat: "The {0} is missing required reflection configuration.",
        category: "Reflection",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Certain types require specific reflection configuration to function correctly. This diagnostic indicates that such configuration is missing.");

    public void Register(AnalysisContext context)
    {
    }
}