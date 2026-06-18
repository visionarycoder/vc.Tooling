using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Architecture.Rules;

internal sealed class LayerViolationRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ArchLayeringViolation,
        title: "Layering rule violation",
        messageFormat: "Type '{0}' references '{1}', violating layering rules.",
        category: "Architecture",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterCompilationStartAction(action: startContext =>
        {
            startContext.RegisterSymbolAction(action: AnalyzeNamedType, symbolKinds: SymbolKind.NamedType);
        });
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol typeSymbol)
        {
            return;
        }

        var fromLayer = GetLayer(namespaceName: typeSymbol.ContainingNamespace?.ToDisplayString());
        if (fromLayer == Layer.Unknown)
        {
            return;
        }

        foreach (var member in typeSymbol.GetMembers())
        {
            switch (member)
            {
                case IMethodSymbol method:
                    AnalyzeTypeDependency(context: context, fromType: typeSymbol, toType: method.ReturnType, fromLayer: fromLayer);
                    foreach (var parameter in method.Parameters)
                    {
                        AnalyzeTypeDependency(context: context, fromType: typeSymbol, toType: parameter.Type, fromLayer: fromLayer);
                    }
                    break;
                case IPropertySymbol property:
                    AnalyzeTypeDependency(context: context, fromType: typeSymbol, toType: property.Type, fromLayer: fromLayer);
                    break;
                case IFieldSymbol field:
                    AnalyzeTypeDependency(context: context, fromType: typeSymbol, toType: field.Type, fromLayer: fromLayer);
                    break;
            }
        }
    }

    private static void AnalyzeTypeDependency(SymbolAnalysisContext context, INamedTypeSymbol fromType, ITypeSymbol toType, Layer fromLayer)
    {
        var namedToType = toType as INamedTypeSymbol ?? toType?.OriginalDefinition as INamedTypeSymbol;
        if (namedToType is null)
        {
            return;
        }

        var toLayer = GetLayer(namespaceName: namedToType.ContainingNamespace?.ToDisplayString());
        if (toLayer == Layer.Unknown || IsAllowedDependency(from: fromLayer, to: toLayer))
        {
            return;
        }

        context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: fromType.Locations.FirstOrDefault(), messageArgs: [fromType.Name, namedToType.Name]));
    }

    private static Layer GetLayer(string? namespaceName)
    {
        if (string.IsNullOrEmpty(value: namespaceName))
        {
            return Layer.Unknown;
        }

        if (namespaceName.Contains(value: ".Api", comparisonType: System.StringComparison.Ordinal))
        {
            return Layer.Api;
        }

        if (namespaceName.Contains(value: ".Application", comparisonType: System.StringComparison.Ordinal))
        {
            return Layer.Application;
        }

        if (namespaceName.Contains(value: ".Domain", comparisonType: System.StringComparison.Ordinal))
        {
            return Layer.Domain;
        }

        if (namespaceName.Contains(value: ".Infrastructure", comparisonType: System.StringComparison.Ordinal))
        {
            return Layer.Infrastructure;
        }

        return Layer.Unknown;
    }

    private static bool IsAllowedDependency(Layer from, Layer to)
    {
        if (from == to)
        {
            return true;
        }

        return (from, to) switch
        {
            (Layer.Api, Layer.Application) => true,
            (Layer.Api, Layer.Domain) => true,
            (Layer.Application, Layer.Domain) => true,
            (Layer.Infrastructure, Layer.Domain) => true,
            _ => false
        };
    }

    private enum Layer
    {
        Unknown,
        Api,
        Application,
        Domain,
        Infrastructure
    }
}