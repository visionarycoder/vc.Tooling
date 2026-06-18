using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Design.Vbd.Rules;

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
        context.RegisterSyntaxNodeAction(action: AnalyzeInvocation, syntaxKinds: Microsoft.CodeAnalysis.CSharp.SyntaxKind.InvocationExpression);
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
        if (containingType == null || !containingType.Name.EndsWith(value: "Engine"))
        {
            return;
        }

        var symbol = context.SemanticModel.GetSymbolInfo(expression: invocation, cancellationToken: context.CancellationToken).Symbol as IMethodSymbol;
        if (symbol == null)
        {
            return;
        }

        var fullName = symbol.ToDisplayString();
        if (fullName.Contains(value: "System.Guid.NewGuid") || fullName.Contains(value: "System.DateTime.Now") || fullName.Contains(value: "System.Random.Next"))
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: invocation.GetLocation(), messageArgs: [methodDecl.Identifier.ValueText, symbol.Name]));
        }
    }
}

