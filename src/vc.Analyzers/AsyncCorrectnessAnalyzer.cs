using System;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vc.Analyzers.Core;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class AsyncCorrectnessAnalyzer : DiagnosticAnalyzer
{
    public const string BlockingCallId = "VCASYNC001";
    public const string FireAndForgetId = "VCASYNC002";
    public const string MissingAsyncSuffixId = "VCASYNC003";

    private static readonly DiagnosticDescriptor BlockingCallRule = new(
        id: BlockingCallId,
        title: "Avoid blocking calls in async methods",
        messageFormat: "Avoid blocking call '{0}' inside an async method; use await instead.",
        category: "AsyncCorrectness",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor FireAndForgetRule = new(
        id: FireAndForgetId,
        title: "Avoid fire-and-forget tasks",
        messageFormat: "Task-returning call '{0}' is not awaited or observed.",
        category: "AsyncCorrectness",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly DiagnosticDescriptor MissingAsyncSuffixRule = new(
        id: MissingAsyncSuffixId,
        title: "Async method should end with 'Async'",
        messageFormat: "Async method '{0}' should be suffixed with 'Async'.",
        category: "AsyncCorrectness",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(BlockingCallRule, FireAndForgetRule, MissingAsyncSuffixRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;

        if (!method.Modifiers.Any(SyntaxKind.AsyncKeyword))
        {
            return;
        }

        var name = method.Identifier.Text;
        if (!name.EndsWith("Async", StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(MissingAsyncSuffixRule, method.Identifier.GetLocation(), name));
        }
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        var semanticModel = context.SemanticModel;
        var cancellationToken = context.CancellationToken;

        var methodSymbol = semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol as IMethodSymbol;
        if (methodSymbol is null)
        {
            return;
        }

        // Only consider invocations inside methods
        var containingMethod = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (containingMethod is null)
        {
            return;
        }

        var isAsyncMethod = containingMethod.Modifiers.Any(SyntaxKind.AsyncKeyword);

        // Detect blocking calls in async methods
        if (isAsyncMethod && IsBlockingCall(methodSymbol))
        {
            context.ReportDiagnostic(Diagnostic.Create(BlockingCallRule, invocation.GetLocation(), methodSymbol.Name));
        }

        // Detect fire-and-forget: Task-returning call whose result is not awaited or assigned
        if (ReturnsTask(methodSymbol) && !IsAwaited(invocation) && !IsAssigned(invocation))
        {
            context.ReportDiagnostic(Diagnostic.Create(FireAndForgetRule, invocation.GetLocation(), methodSymbol.Name));
        }
    }

    private static bool IsBlockingCall(IMethodSymbol methodSymbol)
    {
        if (methodSymbol.Name is "Wait" or "GetResult")
        {
            return true;
        }

        if (methodSymbol.Name == "get_Result" && methodSymbol.ContainingType.Name.Contains("Task", StringComparison.Ordinal))
        {
            return true;
        }

        return false;
    }

    private static bool ReturnsTask(IMethodSymbol methodSymbol)
    {
        var returnType = methodSymbol.ReturnType;
        if (returnType is null)
        {
            return false;
        }

        var name = returnType.Name;
        return name is "Task" or "ValueTask";
    }

    private static bool IsAwaited(InvocationExpressionSyntax invocation)
    {
        return invocation.Parent is AwaitExpressionSyntax;
    }

    private static bool IsAssigned(InvocationExpressionSyntax invocation)
    {
        return invocation.Parent is AssignmentExpressionSyntax or EqualsValueClauseSyntax or ArgumentSyntax;
    }
}
