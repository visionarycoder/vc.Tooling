using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Design.Rules;

internal sealed class AsyncVoidRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.AsyncVoidMethod,
        title: "Avoid async void",
        messageFormat: "Async void methods should be avoided except for event handlers.",
        category: "ExceptionSafety",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(action: AnalyzeMethodDeclaration, syntaxKinds: SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (!method.Modifiers.Any(kind: SyntaxKind.AsyncKeyword))
        {
            return;
        }

        if (method.ReturnType is not PredefinedTypeSyntax predefinedType || !predefinedType.Keyword.IsKind(kind: SyntaxKind.VoidKeyword))
        {
            return;
        }

        if (!IsEventHandlerSignature(semanticModel: context.SemanticModel, method: method, cancellationToken: context.CancellationToken))
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: method.Identifier.GetLocation()));
        }
    }

    private static bool IsEventHandlerSignature(SemanticModel semanticModel, MethodDeclarationSyntax method, System.Threading.CancellationToken cancellationToken)
    {
        var parameters = method.ParameterList.Parameters;
        if (parameters.Count != 2)
        {
            return false;
        }

        var firstType = semanticModel.GetTypeInfo(expression: parameters[index: 0].Type!, cancellationToken: cancellationToken).Type;
        var secondType = semanticModel.GetTypeInfo(expression: parameters[index: 1].Type!, cancellationToken: cancellationToken).Type;
        return firstType?.SpecialType == SpecialType.System_Object && secondType is not null && InheritsFromEventArgs(type: secondType);
    }

    private static bool InheritsFromEventArgs(ITypeSymbol type)
    {
        while (type is not null)
        {
            if (type.ToDisplayString(format: SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.EventArgs")
            {
                return true;
            }

            type = type.BaseType!;
        }

        return false;
    }
}