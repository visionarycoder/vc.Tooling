using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Rules;

internal sealed class SerializationRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.LegacySerialization,
        "Missing serialization configuration",
        "The {0} is missing required serialization configuration.",
        "Serialization",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Certain types require specific serialization configuration to function correctly. This diagnostic indicates that such configuration is missing.");

    public void Register(AnalysisContext context)
    {
    }
}