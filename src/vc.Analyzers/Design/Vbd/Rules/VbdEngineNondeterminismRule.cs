using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Vbd.Rules;

internal sealed class VbdEngineNondeterminismRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.VbdEngineVaultNondeterminism,
        title: "Engine contains nondeterministic operation",
        messageFormat: "Engine method '{0}' uses nondeterministic API '{1}'.",
        category: "VbdEngine",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, Microsoft.CodeAnalysis.CSharp.SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        var methodDecl = invocation.FirstAncestorOrSelf<MethodDeclarationSyntax>();
        if (methodDecl == null)
        {
            return;
        }

        var containingType = context.ContainingSymbol?.ContainingType;
        if (containingType == null || !containingType.Name.EndsWith("Engine"))
        {
            return;
        }

        var symbol = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol as IMethodSymbol;
        if (symbol == null)
        {
            return;
        }

        var fullName = symbol.ToDisplayString();
        if (fullName.Contains("System.Guid.NewGuid") || fullName.Contains("System.DateTime.Now") || fullName.Contains("System.Random.Next"))
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, invocation.GetLocation(), methodDecl.Identifier.ValueText, symbol.Name));
        }
    }
}

