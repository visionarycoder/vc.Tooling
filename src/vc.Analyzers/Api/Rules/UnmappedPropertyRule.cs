using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Api.Rules;

internal sealed class UnmappedPropertyRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.MappingUnmappedProperties,
        "Unmapped property",
        "Property '{0}' is not mapped in method '{1}'.",
        "ApiDesign",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MethodDeclarationSyntax methodSyntax || methodSyntax.Body is null)
        {
            return;
        }

        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(methodSyntax, context.CancellationToken);
        if (methodSymbol is null || !IsMappingMethod(methodSymbol) || methodSymbol.ReturnType is not INamedTypeSymbol returnType)
        {
            return;
        }

        var objectCreation = methodSyntax.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().FirstOrDefault();
        if (objectCreation is null)
        {
            return;
        }

        var mappedProperties = objectCreation.Initializer?.Expressions
            .OfType<AssignmentExpressionSyntax>()
            .Select(assignment => assignment.Left)
            .OfType<IdentifierNameSyntax>()
            .Select(identifier => identifier.Identifier.Text);

        var mappedPropertySet = mappedProperties is null ? new HashSet<string>() : new HashSet<string>(mappedProperties);

        foreach (var property in returnType.GetMembers().OfType<IPropertySymbol>())
        {
            if (!property.IsStatic && !mappedPropertySet.Contains(property.Name))
            {
                context.ReportDiagnostic(Diagnostic.Create(descriptor, methodSyntax.Identifier.GetLocation(), property.Name, methodSymbol.Name));
            }
        }
    }

    private static bool IsMappingMethod(IMethodSymbol methodSymbol)
    {
        if (methodSymbol.MethodKind != MethodKind.Ordinary || methodSymbol.ReturnsVoid)
        {
            return false;
        }

        if (methodSymbol.Name is "ToDto" or "ToEntity" or "Map" or "Convert")
        {
            return true;
        }

        return methodSymbol.ContainingType is not null &&
               !SymbolEqualityComparer.Default.Equals(methodSymbol.ReturnType, methodSymbol.ContainingType);
    }
}