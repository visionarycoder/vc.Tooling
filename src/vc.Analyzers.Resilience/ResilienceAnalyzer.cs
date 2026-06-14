using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vc.Analyzers.Resilience;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ResilienceAnalyzer : DiagnosticAnalyzer
{
    public const string MissingResilienceId = "VCRES009";
    public const string MissingCancellationTokenId = "VCRES010";
    public const string BlockingCallInAsyncId = "VCRES011";

    private static readonly DiagnosticDescriptor MissingResilienceRule = new(
        MissingResilienceId,
        "Missing resilience policy",
        "External call '{0}' is not wrapped in any resilience policy (retry, timeout, circuit breaker, fallback).",
        "Resilience",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MissingCancellationTokenRule = new(
        MissingCancellationTokenId,
        "Missing cancellation token",
        "Async external call '{0}' does not accept or pass a CancellationToken.",
        "Resilience",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor BlockingCallInAsyncRule = new(
        BlockingCallInAsyncId,
        "Blocking call inside async method",
        "Blocking call '{0}' inside async method can cause thread starvation.",
        "Resilience",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MissingResilienceRule, MissingCancellationTokenRule, BlockingCallInAsyncRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        var semanticModel = context.SemanticModel;
        var cancellationToken = context.CancellationToken;

        var symbol = semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol as IMethodSymbol;
        if (symbol is null)
        {
            return;
        }

        if (!IsExternalCall(symbol))
        {
            return;
        }

        var method = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (method is null)
        {
            return;
        }

        var methodSymbol = semanticModel.GetDeclaredSymbol(method, cancellationToken);
        if (methodSymbol is null)
        {
            return;
        }

        if (!IsInsideResiliencePolicy(invocation))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    MissingResilienceRule,
                    invocation.GetLocation(),
                    symbol.Name));
        }

        if (methodSymbol.IsAsync && !HasCancellationToken(methodSymbol))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    MissingCancellationTokenRule,
                    invocation.GetLocation(),
                    symbol.Name));
        }

        if (methodSymbol.IsAsync && IsBlockingCall(invocation))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    BlockingCallInAsyncRule,
                    invocation.GetLocation(),
                    invocation.ToString()));
        }
    }

    private static bool IsExternalCall(IMethodSymbol methodSymbol)
    {
        var ns = methodSymbol.ContainingNamespace?.ToDisplayString();

        if (ns is null)
        {
            return false;
        }

        if (ns.StartsWith("System.Net.Http"))
        {
            return true;
        }

        if (ns.Contains("SqlClient"))
        {
            return true;
        }

        if (ns.Contains("Redis") || ns.Contains("Mongo") || ns.Contains("Cosmos"))
        {
            return true;
        }

        return false;
    }

    private static bool IsInsideResiliencePolicy(SyntaxNode node)
    {
        var current = node.Parent;

        while (current is not null)
        {
            if (current is InvocationExpressionSyntax invocation)
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    var name = memberAccess.Name.Identifier.Text;

                    if (name.Contains("Retry") ||
                        name.Contains("Timeout") ||
                        name.Contains("CircuitBreaker") ||
                        name.Contains("Fallback") ||
                        name.Contains("Execute") ||
                        name.Contains("Wrap"))
                    {
                        return true;
                    }
                }
            }

            current = current.Parent;
        }

        return false;
    }

    private static bool HasCancellationToken(IMethodSymbol methodSymbol)
    {
        return methodSymbol.Parameters.Any(p => p.Type.Name == "CancellationToken");
    }

    private static bool IsBlockingCall(InvocationExpressionSyntax invocation)
    {
        if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
        {
            var name = memberAccess.Name.Identifier.Text;

            if (name is "Wait" or "Result" or "GetAwaiter")
            {
                return true;
            }
        }

        return false;
    }
}
