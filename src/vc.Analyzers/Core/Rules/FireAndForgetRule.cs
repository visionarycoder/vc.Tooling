using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Core.Rules;

internal sealed class FireAndForgetRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.AsyncFireAndForget,
        title: "Avoid fire-and-forget tasks",
        messageFormat: "Task-returning call '{0}' is not awaited or observed.",
        category: "AsyncCorrectness",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(action: AnalyzeInvocation, syntaxKinds: SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        var methodSymbol = context.SemanticModel.GetSymbolInfo(expression: invocation, cancellationToken: context.CancellationToken).Symbol as IMethodSymbol;
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

        context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: invocation.GetLocation(), messageArgs: methodSymbol.Name));
    }
}