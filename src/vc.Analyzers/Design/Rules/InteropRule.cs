using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Rules;

internal sealed class InteropRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        "VC0006",
        "Missing interop configuration",
        "The {0} is missing required interop configuration.",
        "Interop",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Certain types require specific interop configuration to function correctly. This diagnostic indicates that such configuration is missing.");

    public void Register(AnalysisContext context)
    {
    }
}