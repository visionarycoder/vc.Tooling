using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Rules;

internal sealed class AsyncVoidRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.AsyncVoidMethod,
        "Avoid async void",
        "Async void methods should be avoided except for event handlers.",
        "ExceptionSafety",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (!method.Modifiers.Any(SyntaxKind.AsyncKeyword))
        {
            return;
        }

        if (method.ReturnType is not PredefinedTypeSyntax predefinedType || !predefinedType.Keyword.IsKind(SyntaxKind.VoidKeyword))
        {
            return;
        }

        if (!IsEventHandlerSignature(context.SemanticModel, method, context.CancellationToken))
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, method.Identifier.GetLocation()));
        }
    }

    private static bool IsEventHandlerSignature(SemanticModel semanticModel, MethodDeclarationSyntax method, System.Threading.CancellationToken cancellationToken)
    {
        var parameters = method.ParameterList.Parameters;
        if (parameters.Count != 2)
        {
            return false;
        }

        var firstType = semanticModel.GetTypeInfo(parameters[0].Type!, cancellationToken).Type;
        var secondType = semanticModel.GetTypeInfo(parameters[1].Type!, cancellationToken).Type;
        return firstType?.SpecialType == SpecialType.System_Object && secondType is not null && InheritsFromEventArgs(secondType);
    }

    private static bool InheritsFromEventArgs(ITypeSymbol type)
    {
        while (type is not null)
        {
            if (type.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat) == "global::System.EventArgs")
            {
                return true;
            }

            type = type.BaseType!;
        }

        return false;
    }
}