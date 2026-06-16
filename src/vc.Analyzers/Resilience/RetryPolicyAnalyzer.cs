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
