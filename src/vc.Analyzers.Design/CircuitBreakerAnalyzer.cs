using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vc.Analyzers.Distributed;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CircuitBreakerAnalyzer : DiagnosticAnalyzer
{
    public const string MissingCircuitBreakerId = "VCRES001";
    public const string UnusedCircuitBreakerId = "VCRES002";

    private static readonly DiagnosticDescriptor MissingCircuitBreakerRule = new(
        MissingCircuitBreakerId,
        "Missing circuit breaker",
        "External call '{0}' is not wrapped in a circuit breaker policy.",
        "Resilience",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor UnusedCircuitBreakerRule = new(
        UnusedCircuitBreakerId,
        "Circuit breaker declared but never used",
        "Circuit breaker policy '{0}' is declared but never applied.",
        "Resilience",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MissingCircuitBreakerRule, UnusedCircuitBreakerRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
        context.RegisterSymbolAction(AnalyzeFieldOrProperty, SymbolKind.Field, SymbolKind.Property);
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

        if (IsInsideCircuitBreaker(invocation))
        {
            return;
        }

        context.ReportDiagnostic(
            Diagnostic.Create(
                MissingCircuitBreakerRule,
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

    private static bool IsInsideCircuitBreaker(SyntaxNode node)
    {
        var current = node.Parent;

        while (current is not null)
        {
            if (current is InvocationExpressionSyntax invocation)
            {
                if (invocation.Expression is MemberAccessExpressionSyntax memberAccess)
                {
                    var name = memberAccess.Name.Identifier.Text;

                    if (name.Contains("ExecuteAsync") ||
                        name.Contains("Execute") ||
                        name.Contains("WrapAsync") ||
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

    private static void AnalyzeFieldOrProperty(SymbolAnalysisContext context)
    {
        var symbol = context.Symbol;

        if (!IsCircuitBreakerPolicy(symbol))
        {
            return;
        }

        if (!IsUsed(symbol))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    UnusedCircuitBreakerRule,
                    symbol.Locations.FirstOrDefault(),
                    symbol.Name));
        }
    }

    private static bool IsCircuitBreakerPolicy(ISymbol symbol)
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

        return type.Name.Contains("CircuitBreaker") ||
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
}
