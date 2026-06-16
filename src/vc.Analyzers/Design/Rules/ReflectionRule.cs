using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Rules;

internal sealed class ReflectionRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        "VC0005",
        "Missing reflection configuration",
        "The {0} is missing required reflection configuration.",
        "Reflection",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Certain types require specific reflection configuration to function correctly. This diagnostic indicates that such configuration is missing.");

    public void Register(AnalysisContext context)
    {
    }
}