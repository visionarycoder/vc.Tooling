using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Rules;

internal sealed class DependencyInjectionRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.LegacyDependencyInjection,
        "Missing dependency injection configuration",
        "The {0} is missing required dependency injection configuration.",
        "DependencyInjection",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true,
        description: "Certain types require specific dependency injection configuration to function correctly. This diagnostic indicates that such configuration is missing.");

    public void Register(AnalysisContext context)
    {
    }
}