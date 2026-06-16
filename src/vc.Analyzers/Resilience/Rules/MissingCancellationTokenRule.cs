using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Resilience.Rules;

internal sealed class MissingCancellationTokenRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.ResilienceCancellationTokenMissing,
        "Missing cancellation token",
        "Async external call '{0}' does not accept or pass a CancellationToken.",
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

        var semanticModel = context.SemanticModel;
        var symbol = semanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol as IMethodSymbol;
        if (symbol is null || !IsExternalCall(symbol))
        {
            return;
        }

        var methodSyntax = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        var methodSymbol = methodSyntax is null ? null : semanticModel.GetDeclaredSymbol(methodSyntax, context.CancellationToken);
        if (methodSymbol?.IsAsync == true && !methodSymbol.Parameters.Any(parameter => parameter.Type.Name == "CancellationToken"))
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, invocation.GetLocation(), symbol.Name));
        }
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
}