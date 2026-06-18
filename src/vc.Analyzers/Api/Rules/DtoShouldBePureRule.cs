using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Api.Rules;

internal sealed class DtoShouldBePureRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.DtoMutableState,
        title: "DTO should be pure data",
        messageFormat: "DTO '{0}' contains behavior or mutable state",
        category: "ApiDesign",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(action: AnalyzeType, symbolKinds: SymbolKind.NamedType);
    }

    private static void AnalyzeType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol typeSymbol || !IsDto(typeSymbol: typeSymbol))
        {
            return;
        }

        if (!HasBehavior(typeSymbol: typeSymbol) && !HasMutableState(typeSymbol: typeSymbol))
        {
            return;
        }

        context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: typeSymbol.Locations.FirstOrDefault(), messageArgs: typeSymbol.Name));
    }

    private static bool IsDto(INamedTypeSymbol typeSymbol)
    {
        return (typeSymbol.TypeKind == TypeKind.Class || typeSymbol.TypeKind == TypeKind.Struct) &&
               typeSymbol.Name.EndsWith(value: "Dto", comparisonType: System.StringComparison.Ordinal);
    }

    private static bool HasBehavior(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers().OfType<IMethodSymbol>().Any(predicate: method =>
            method.MethodKind == MethodKind.Ordinary &&
            !method.IsImplicitlyDeclared &&
            !IsTrivialMethod(method: method));
    }

    private static bool IsTrivialMethod(IMethodSymbol method)
    {
        return method.Name is "ToString" or "Equals" or "GetHashCode" ||
               method.MethodKind is MethodKind.PropertyGet or MethodKind.PropertySet;
    }

    private static bool HasMutableState(INamedTypeSymbol typeSymbol)
    {
        foreach (var member in typeSymbol.GetMembers())
        {
            if (member is IPropertySymbol property && !property.IsReadOnly)
            {
                return true;
            }

            if (member is IFieldSymbol field && !field.IsReadOnly)
            {
                return true;
            }
        }

        return false;
    }
}