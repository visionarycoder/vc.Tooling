using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Api.Rules;

internal sealed class DtoShouldBePureRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.DtoMutableState,
        "DTO should be pure data",
        "DTO '{0}' contains behavior or mutable state.",
        "ApiDesign",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
    }

    private static void AnalyzeType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol typeSymbol || !IsDto(typeSymbol))
        {
            return;
        }

        if (!HasBehavior(typeSymbol) && !HasMutableState(typeSymbol))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(descriptor, typeSymbol.Locations.FirstOrDefault(), typeSymbol.Name));
    }

    private static bool IsDto(INamedTypeSymbol typeSymbol)
    {
        return (typeSymbol.TypeKind == TypeKind.Class || typeSymbol.TypeKind == TypeKind.Struct) &&
               typeSymbol.Name.EndsWith("Dto", System.StringComparison.Ordinal);
    }

    private static bool HasBehavior(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol.GetMembers().OfType<IMethodSymbol>().Any(method =>
            method.MethodKind == MethodKind.Ordinary &&
            !method.IsImplicitlyDeclared &&
            !IsTrivialMethod(method));
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