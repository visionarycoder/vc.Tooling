using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vc.Analyzers.Api;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class DtoAnalyzer : DiagnosticAnalyzer
{
    public const string DtoShouldBePureId = "VCDTO001";

    private static readonly DiagnosticDescriptor DtoShouldBePureRule = new(
        DtoShouldBePureId,
        "DTO should be pure data",
        "DTO '{0}' contains behavior or mutable state.",
        "ApiDesign",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(DtoShouldBePureRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
    }

    private static void AnalyzeType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol typeSymbol)
        {
            return;
        }

        if (!IsDto(typeSymbol))
        {
            return;
        }

        var hasBehavior = HasBehavior(typeSymbol);
        var hasMutableState = HasMutableState(typeSymbol);

        if (!hasBehavior && !hasMutableState)
        {
            return;
        }

        context.ReportDiagnostic(
            Diagnostic.Create(
                DtoShouldBePureRule,
                typeSymbol.Locations.FirstOrDefault(),
                typeSymbol.Name));
    }

    private static bool IsDto(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol.TypeKind != TypeKind.Class && typeSymbol.TypeKind != TypeKind.Struct)
        {
            return false;
        }

        if (!typeSymbol.Name.EndsWith("Dto", System.StringComparison.Ordinal))
        {
            return false;
        }

        return true;
    }

    private static bool HasBehavior(INamedTypeSymbol typeSymbol)
    {
        foreach (var member in typeSymbol.GetMembers())
        {
            if (member is IMethodSymbol method &&
                method.MethodKind == MethodKind.Ordinary &&
                !method.IsImplicitlyDeclared &&
                !IsTrivialMethod(method))
            {
                return true;
            }
        }

        return false;
    }

    private static bool IsTrivialMethod(IMethodSymbol method)
    {
        if (method.Name is "ToString" or "Equals" or "GetHashCode")
        {
            return true;
        }

        if (method.MethodKind == MethodKind.PropertyGet || method.MethodKind == MethodKind.PropertySet)
        {
            return true;
        }

        return false;
    }

    private static bool HasMutableState(INamedTypeSymbol typeSymbol)
    {
        foreach (var member in typeSymbol.GetMembers())
        {
            if (member is IPropertySymbol property)
            {
                if (!property.IsReadOnly)
                {
                    return true;
                }
            }

            if (member is IFieldSymbol field)
            {
                if (!field.IsReadOnly)
                {
                    return true;
                }
            }
        }

        return false;
    }
}
