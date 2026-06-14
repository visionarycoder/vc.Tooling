using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vc.Analyzers.Resilience;

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
