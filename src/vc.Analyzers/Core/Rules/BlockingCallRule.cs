using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Core.Rules;

internal sealed class BlockingCallRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.AsyncBlockingCall,
        "Avoid blocking calls in async methods",
        "Avoid blocking call '{0}' inside an async method; use await instead.",
        "AsyncCorrectness",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        var methodSymbol = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol as IMethodSymbol;
        var containingMethod = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (methodSymbol is null || containingMethod is null || !containingMethod.Modifiers.Any(SyntaxKind.AsyncKeyword))
        {
            return;
        }

        if (methodSymbol.Name is "Wait" or "GetResult" ||
            (methodSymbol.Name == "get_Result" && methodSymbol.ContainingType.Name.Contains("Task", StringComparison.Ordinal)))
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, invocation.GetLocation(), methodSymbol.Name));
        }
    }
}