using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Api.Rules;

internal sealed class UnmappedPropertyRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.MappingUnmappedProperties,
        title: "Unmapped property",
        messageFormat: "Property '{0}' is not mapped in method '{1}'",
        category: "ApiDesign",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(action: AnalyzeMethod, syntaxKinds: SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethod(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not MethodDeclarationSyntax methodSyntax || methodSyntax.Body is null)
        {
            return;
        }

        var methodSymbol = context.SemanticModel.GetDeclaredSymbol(declarationSyntax: methodSyntax, cancellationToken: context.CancellationToken);
        if (methodSymbol is null || !IsMappingMethod(methodSymbol: methodSymbol) || methodSymbol.ReturnType is not INamedTypeSymbol returnType)
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
            .Select(selector: assignment => assignment.Left)
            .OfType<IdentifierNameSyntax>()
            .Select(selector: identifier => identifier.Identifier.Text);

        var mappedPropertySet = mappedProperties is null ? new HashSet<string>() : new HashSet<string>(collection: mappedProperties);

        foreach (var property in returnType.GetMembers().OfType<IPropertySymbol>())
        {
            if (!property.IsStatic && !mappedPropertySet.Contains(item: property.Name))
            {
                context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: methodSyntax.Identifier.GetLocation(), messageArgs: [property.Name, methodSymbol.Name]));
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
               !SymbolEqualityComparer.Default.Equals(x: methodSymbol.ReturnType, y: methodSymbol.ContainingType);
    }
}