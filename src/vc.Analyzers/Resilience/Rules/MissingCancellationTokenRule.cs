using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Resilience.Rules;

internal sealed class MissingCancellationTokenRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ResilienceCancellationTokenMissing,
        title: "Missing cancellation token",
        messageFormat: "Async external call '{0}' does not accept or pass a CancellationToken.",
        category: "Resilience",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(action: AnalyzeInvocation, syntaxKinds: SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        var semanticModel = context.SemanticModel;
        var symbol = semanticModel.GetSymbolInfo(expression: invocation, cancellationToken: context.CancellationToken).Symbol as IMethodSymbol;
        if (symbol is null || !IsExternalCall(methodSymbol: symbol))
        {
            return;
        }

        var methodSyntax = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        var methodSymbol = methodSyntax is null ? null : semanticModel.GetDeclaredSymbol(declarationSyntax: methodSyntax, cancellationToken: context.CancellationToken);
        if (methodSymbol?.IsAsync == true && !methodSymbol.Parameters.Any(predicate: parameter => parameter.Type.Name == "CancellationToken"))
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: invocation.GetLocation(), messageArgs: symbol.Name));
        }
    }

    private static bool IsExternalCall(IMethodSymbol methodSymbol)
    {
        var ns = methodSymbol.ContainingNamespace?.ToDisplayString();
        return ns is not null &&
               (ns.StartsWith(value: "System.Net.Http", comparisonType: System.StringComparison.Ordinal) ||
                ns.Contains(value: "SqlClient", comparisonType: System.StringComparison.Ordinal) ||
                ns.Contains(value: "Redis", comparisonType: System.StringComparison.Ordinal) ||
                ns.Contains(value: "Mongo", comparisonType: System.StringComparison.Ordinal) ||
                ns.Contains(value: "Cosmos", comparisonType: System.StringComparison.Ordinal));
    }
}