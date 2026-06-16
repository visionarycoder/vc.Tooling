using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Core.Rules;

internal sealed class FireAndForgetRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.AsyncFireAndForget,
        "Avoid fire-and-forget tasks",
        "Task-returning call '{0}' is not awaited or observed.",
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
        if (methodSymbol is null)
        {
            return;
        }

        var returnTypeName = methodSymbol.ReturnType?.Name;
        if (returnTypeName is not "Task" and not "ValueTask")
        {
            return;
        }

        if (invocation.Parent is AwaitExpressionSyntax or AssignmentExpressionSyntax or EqualsValueClauseSyntax or ArgumentSyntax)
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(descriptor, invocation.GetLocation(), methodSymbol.Name));
    }
}