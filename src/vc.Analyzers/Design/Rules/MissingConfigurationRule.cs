using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Rules;

internal sealed class MissingConfigurationRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        "VC0002",
        "Missing configuration",
        "The {0} is missing required configuration.",
        "Configuration",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Certain types require specific configuration to function correctly. This diagnostic indicates that such configuration is missing.");

    public void Register(AnalysisContext context)
    {
    }
}