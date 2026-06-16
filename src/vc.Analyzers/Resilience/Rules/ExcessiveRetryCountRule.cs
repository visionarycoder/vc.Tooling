using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Resilience.Rules;

internal sealed class ExcessiveRetryCountRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.ResilienceRetryConfigurationIssue,
        "Retry count too high",
        "Retry policy '{0}' retries {1} times, which exceeds recommended limits.",
        "Resilience",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyzePolicyDeclaration, SymbolKind.Field, SymbolKind.Property);
    }

    private static void AnalyzePolicyDeclaration(SymbolAnalysisContext context)
    {
        var retryCount = GetRetryCount(context.Symbol);
        if (retryCount > 5)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, context.Symbol.Locations.FirstOrDefault(), context.Symbol.Name, retryCount));
        }
    }

    private static int GetRetryCount(ISymbol symbol)
    {
        foreach (var reference in symbol.DeclaringSyntaxReferences)
        {
            foreach (var invocation in reference.GetSyntax().SyntaxTree.GetRoot().DescendantNodes().OfType<InvocationExpressionSyntax>())
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    var name = memberAccess.Name.Identifier.Text;
                    if ((name.Contains("Retry", System.StringComparison.Ordinal) || name.Contains("WaitAndRetry", System.StringComparison.Ordinal)) &&
                        invocation.ArgumentList.Arguments.Count > 0 &&
                        invocation.ArgumentList.Arguments[0].Expression is LiteralExpressionSyntax literal &&
                        literal.Token.Value is int retryCount)
                    {
                        return retryCount;
                    }
                }
            }
        }

        return 0;
    }
}