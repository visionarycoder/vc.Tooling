using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Resilience.Rules;

internal sealed class MissingRetryPolicyRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.ResilienceRetryPolicyMissing,
        "Missing retry policy",
        "External call '{0}' is not wrapped in a retry policy.",
        "Resilience",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        var symbol = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol as IMethodSymbol;
        if (symbol is null || !IsExternalCall(symbol) || IsInsideRetryPolicy(invocation))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(descriptor, invocation.GetLocation(), symbol.Name));
    }

    private static bool IsExternalCall(IMethodSymbol methodSymbol)
    {
        var ns = methodSymbol.ContainingNamespace?.ToDisplayString();
        return ns is not null &&
               (ns.StartsWith("System.Net.Http", System.StringComparison.Ordinal) ||
                ns.Contains("SqlClient", System.StringComparison.Ordinal) ||
                ns.Contains("Redis", System.StringComparison.Ordinal) ||
                ns.Contains("Mongo", System.StringComparison.Ordinal) ||
                ns.Contains("Cosmos", System.StringComparison.Ordinal));
    }

    private static bool IsInsideRetryPolicy(SyntaxNode node)
    {
        var current = node.Parent;
        while (current is not null)
        {
            if (current is InvocationExpressionSyntax invocation && invocation.Expression is MemberAccessExpressionSyntax memberAccess)
            {
                var name = memberAccess.Name.Identifier.Text;
                if (name.Contains("Retry", System.StringComparison.Ordinal) ||
                    name.Contains("WaitAndRetry", System.StringComparison.Ordinal) ||
                    name.Contains("RetryAsync", System.StringComparison.Ordinal) ||
                    name.Contains("WaitAndRetryAsync", System.StringComparison.Ordinal))
                {
                    return true;
                }
            }

            current = current.Parent;
        }

        return false;
    }
}