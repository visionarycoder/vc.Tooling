using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vc.Analyzers.Resilience;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class RetryPolicyAnalyzer : DiagnosticAnalyzer
{
    public const string MissingRetryPolicyId = "VCRES003";
    public const string UnusedRetryPolicyId = "VCRES004";
    public const string ExcessiveRetryCountId = "VCRES005";

    private static readonly DiagnosticDescriptor MissingRetryPolicyRule = new(
        MissingRetryPolicyId,
        "Missing retry policy",
        "External call '{0}' is not wrapped in a retry policy.",
        "Resilience",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor UnusedRetryPolicyRule = new(
        UnusedRetryPolicyId,
        "Retry policy declared but never used",
        "Retry policy '{0}' is declared but never applied.",
        "Resilience",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor ExcessiveRetryCountRule = new(
        ExcessiveRetryCountId,
        "Retry count too high",
        "Retry policy '{0}' retries {1} times, which exceeds recommended limits.",
        "Resilience",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MissingRetryPolicyRule, UnusedRetryPolicyRule, ExcessiveRetryCountRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        context.RegisterSymbolAction(AnalyzePolicyDeclaration, SymbolKind.Field, SymbolKind.Property);
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

        if (IsInsideRetryPolicy(invocation))
        {
            return;
        }

        context.ReportDiagnostic(
            Diagnostic.Create(
                MissingRetryPolicyRule,
                invocation.GetLocation(),
                symbol.Name));
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

    private static bool IsInsideRetryPolicy(SyntaxNode node)
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
                        name.Contains("WaitAndRetry") ||
                        name.Contains("RetryAsync") ||
                        name.Contains("WaitAndRetryAsync"))
                    {
                        return true;
                    }
                }
            }

            current = current.Parent;
        }

        return false;
    }

    private static void AnalyzePolicyDeclaration(SymbolAnalysisContext context)
    {
        var symbol = context.Symbol;

        if (!IsRetryPolicy(symbol))
        {
            return;
        }

        if (!IsUsed(symbol))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    UnusedRetryPolicyRule,
                    symbol.Locations.FirstOrDefault(),
                    symbol.Name));
        }

        var retryCount = GetRetryCount(symbol);
        if (retryCount > 5)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    ExcessiveRetryCountRule,
                    symbol.Locations.FirstOrDefault(),
                    symbol.Name,
                    retryCount));
        }
    }

    private static bool IsRetryPolicy(ISymbol symbol)
    {
        var type = symbol switch
        {
            IFieldSymbol f => f.Type,
            IPropertySymbol p => p.Type,
            _ => null
        };

        if (type is null)
        {
            return false;
        }

        return type.Name.Contains("Retry") ||
               type.Name.Contains("AsyncPolicy") ||
               type.Name.Contains("Policy");
    }

    private static bool IsUsed(ISymbol symbol)
    {
        foreach (var reference in symbol.DeclaringSyntaxReferences)
        {
            var syntax = reference.GetSyntax();

            var root = syntax.SyntaxTree.GetRoot();

            var invocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>();

            foreach (var invocation in invocations)
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    if (memberAccess.Expression is IdentifierNameSyntax id &&
                        id.Identifier.Text == symbol.Name)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private static int GetRetryCount(ISymbol symbol)
    {
        foreach (var reference in symbol.DeclaringSyntaxReferences)
        {
            var syntax = reference.GetSyntax();

            var invocations = syntax.SyntaxTree
                .GetRoot()
                .DescendantNodes()
                .OfType<InvocationExpressionSyntax>();

            foreach (var invocation in invocations)
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    var name = memberAccess.Name.Identifier.Text;

                    if (name.Contains("Retry") || name.Contains("WaitAndRetry"))
                    {
                        var args = invocation.ArgumentList.Arguments;

                        if (args.Count > 0 &&
                            args[0].Expression is LiteralExpressionSyntax literal &&
                            literal.Token.Value is int retryCount)
                        {
                            return retryCount;
                        }
                    }
                }
            }
        }

        return 0;
    }
}

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class TimeoutAnalyzer : DiagnosticAnalyzer
{
    public const string MissingTimeoutId = "VCRES006";
    public const string UnusedTimeoutId = "VCRES007";
    public const string ExcessiveTimeoutId = "VCRES008";

    private static readonly DiagnosticDescriptor MissingTimeoutRule = new(
        MissingTimeoutId,
        "Missing timeout",
        "External call '{0}' is not wrapped in a timeout or does not use a cancellation token.",
        "Resilience",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor UnusedTimeoutRule = new(
        UnusedTimeoutId,
        "Timeout declared but never used",
        "Timeout value '{0}' is declared but never applied to any external call.",
        "Resilience",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor ExcessiveTimeoutRule = new(
        ExcessiveTimeoutId,
        "Timeout value too high",
        "Timeout '{0}' is set to {1} seconds, which exceeds recommended limits.",
        "Resilience",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MissingTimeoutRule, UnusedTimeoutRule, ExcessiveTimeoutRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        context.RegisterSymbolAction(AnalyzeTimeoutDeclaration, SymbolKind.Field, SymbolKind.Property);
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

        if (IsInsideTimeout(invocation))
        {
            return;
        }

        if (HasCancellationToken(invocation, symbol))
        {
            return;
        }

        context.ReportDiagnostic(
            Diagnostic.Create(
                MissingTimeoutRule,
                invocation.GetLocation(),
                symbol.Name));
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

    private static bool IsInsideTimeout(SyntaxNode node)
    {
        var current = node.Parent;

        while (current is not null)
        {
            if (current is InvocationExpressionSyntax invocation)
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    var name = memberAccess.Name.Identifier.Text;

                    if (name.Contains("Timeout") ||
                        name.Contains("WithTimeout") ||
                        name.Contains("TimeoutAfter"))
                    {
                        return true;
                    }
                }
            }

            current = current.Parent;
        }

        return false;
    }

    private static bool HasCancellationToken(InvocationExpressionSyntax invocation, IMethodSymbol methodSymbol)
    {
        foreach (var parameter in methodSymbol.Parameters)
        {
            if (parameter.Type.Name == "CancellationToken")
            {
                return true;
            }
        }

        return false;
    }

    private static void AnalyzeTimeoutDeclaration(SymbolAnalysisContext context)
    {
        var symbol = context.Symbol;

        if (!IsTimeoutValue(symbol))
        {
            return;
        }

        if (!IsUsed(symbol))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    UnusedTimeoutRule,
                    symbol.Locations.FirstOrDefault(),
                    symbol.Name));
        }

        var seconds = GetTimeoutSeconds(symbol);
        if (seconds > 30)
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    ExcessiveTimeoutRule,
                    symbol.Locations.FirstOrDefault(),
                    symbol.Name,
                    seconds));
        }
    }

    private static bool IsTimeoutValue(ISymbol symbol)
    {
        var type = symbol switch
        {
            IFieldSymbol f => f.Type,
            IPropertySymbol p => p.Type,
            _ => null
        };

        if (type is null)
        {
            return false;
        }

        return type.Name.Contains("TimeSpan") ||
               symbol.Name.Contains("Timeout") ||
               symbol.Name.Contains("Delay");
    }

    private static bool IsUsed(ISymbol symbol)
    {
        foreach (var reference in symbol.DeclaringSyntaxReferences)
        {
            var syntax = reference.GetSyntax();

            var root = syntax.SyntaxTree.GetRoot();

            var invocations = root.DescendantNodes().OfType<InvocationExpressionSyntax>();

            foreach (var invocation in invocations)
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    if (memberAccess.Expression is IdentifierNameSyntax id &&
                        id.Identifier.Text == symbol.Name)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    private static int GetTimeoutSeconds(ISymbol symbol)
    {
        foreach (var reference in symbol.DeclaringSyntaxReferences)
        {
            var syntax = reference.GetSyntax();

            var literals = syntax.SyntaxTree
                .GetRoot()
                .DescendantNodes()
                .OfType<LiteralExpressionSyntax>();

            foreach (var literal in literals)
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
