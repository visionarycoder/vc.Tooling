using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Resilience.Rules;

internal sealed class ExcessiveTimeoutRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.ResilienceTimeoutConfigurationIssue,
        "Timeout value too high",
        "Timeout '{0}' is set to {1} seconds, which exceeds recommended limits.",
        "Resilience",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyzeTimeoutDeclaration, SymbolKind.Field, SymbolKind.Property);
    }

    private static void AnalyzeTimeoutDeclaration(SymbolAnalysisContext context)
    {
        var seconds = GetTimeoutSeconds(context.Symbol);
        if (seconds > 30)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, context.Symbol.Locations.FirstOrDefault(), context.Symbol.Name, seconds));
        }
    }

    private static int GetTimeoutSeconds(ISymbol symbol)
    {
        foreach (var reference in symbol.DeclaringSyntaxReferences)
        {
            foreach (var literal in reference.GetSyntax().SyntaxTree.GetRoot().DescendantNodes().OfType<LiteralExpressionSyntax>())
            {
                if (literal.Token.Value is int seconds)
                {
                    return seconds;
                }
            }
        }

        return 0;
    }
}