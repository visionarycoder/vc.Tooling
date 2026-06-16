using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vc.Analyzers.Api;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class MappingAnalyzer : DiagnosticAnalyzer
{
    public const string UnmappedPropertyId = "VCMAP001";

    private static readonly DiagnosticDescriptor UnmappedPropertyRule = new(
        UnmappedPropertyId,
        "Unmapped property",
        "Property '{0}' is not mapped in method '{1}'.",
        "ApiDesign",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(UnmappedPropertyRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeMethod, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MethodDeclarationSyntax methodSyntax)
        {
            return;
        }

        if (methodSyntax.Body is null)
        {
            return;
        }

        var semanticModel = context.SemanticModel;
        var cancellationToken = context.CancellationToken;

        var methodSymbol = semanticModel.GetDeclaredSymbol(methodSyntax, cancellationToken);
        if (methodSymbol is null)
        {
            return;
        }

        if (!IsMappingMethod(methodSymbol))
        {
            return;
        }

        var returnType = methodSymbol.ReturnType as INamedTypeSymbol;
        if (returnType is null)
        {
            return;
        }

        var objectCreation = methodSyntax
            .DescendantNodes()
            .OfType<ObjectCreationExpressionSyntax>()
            .FirstOrDefault();

        if (objectCreation is null)
        {
            return;
        }

        var mappedProperties = objectCreation
            .Initializer?
            .Expressions
            .OfType<AssignmentExpressionSyntax>()
            .Select(a => a.Left)
            .OfType<IdentifierNameSyntax>()
            .Select(id => id.Identifier.Text);

        var mappedPropertySet = mappedProperties is null
            ? new HashSet<string>()
            : new HashSet<string>(mappedProperties);

        foreach (var property in returnType.GetMembers().OfType<IPropertySymbol>())
        {
            if (property.IsStatic)
            {
                continue;
            }

            if (mappedPropertySet.Contains(property.Name))
            {
                continue;
            }

            context.ReportDiagnostic(
                Diagnostic.Create(
                    UnmappedPropertyRule,
                    methodSyntax.Identifier.GetLocation(),
                    property.Name,
                    methodSymbol.Name));
        }
    }

    private static bool IsMappingMethod(IMethodSymbol methodSymbol)
    {
        if (methodSymbol.MethodKind != MethodKind.Ordinary)
        {
            return false;
        }

        if (methodSymbol.ReturnsVoid)
        {
            return false;
        }

        var name = methodSymbol.Name;

        if (name is "ToDto" or "ToEntity" or "Map" or "Convert")
        {
            return true;
        }

        var containingType = methodSymbol.ContainingType;
        if (containingType is null)
        {
            return false;
        }

        if (!SymbolEqualityComparer.Default.Equals(methodSymbol.ReturnType, containingType))
        {
            return true;
        }

        return false;
    }
}