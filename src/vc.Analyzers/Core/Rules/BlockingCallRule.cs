using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Core.Rules;

internal sealed class BlockingCallRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.AsyncBlockingCall,
        title: "Avoid blocking calls in async methods",
        messageFormat: "Avoid blocking call '{0}' inside an async method; use await instead.",
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
        var containingMethod = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (methodSymbol is null || containingMethod is null || !containingMethod.Modifiers.Any(kind: SyntaxKind.AsyncKeyword))
        {
            return;
        }

        if (methodSymbol.Name is "Wait" or "GetResult" ||
            (methodSymbol.Name == "get_Result" && methodSymbol.ContainingType.Name.Contains(value: "Task", comparisonType: StringComparison.Ordinal)))
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: invocation.GetLocation(), messageArgs: methodSymbol.Name));
        }
    }
}