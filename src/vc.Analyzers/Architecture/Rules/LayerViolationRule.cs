using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Architecture.Rules;

internal sealed class LayerViolationRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.ArchLayeringViolation,
        "Layering rule violation",
        "Type '{0}' references '{1}', violating layering rules.",
        "Architecture",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterCompilationStartAction(startContext =>
        {
            startContext.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
        });
    }

    private static void AnalyzeNamedType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol typeSymbol)
        {
            return;
        }

        var fromLayer = GetLayer(typeSymbol.ContainingNamespace?.ToDisplayString());
        if (fromLayer == Layer.Unknown)
        {
            return;
        }

        foreach (var member in typeSymbol.GetMembers())
        {
            switch (member)
            {
                case IMethodSymbol method:
                    AnalyzeTypeDependency(context, typeSymbol, method.ReturnType, fromLayer);
                    foreach (var parameter in method.Parameters)
                    {
                        AnalyzeTypeDependency(context, typeSymbol, parameter.Type, fromLayer);
                    }
                    break;
                case IPropertySymbol property:
                    AnalyzeTypeDependency(context, typeSymbol, property.Type, fromLayer);
                    break;
                case IFieldSymbol field:
                    AnalyzeTypeDependency(context, typeSymbol, field.Type, fromLayer);
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

        var toLayer = GetLayer(namedToType.ContainingNamespace?.ToDisplayString());
        if (toLayer == Layer.Unknown || IsAllowedDependency(fromLayer, toLayer))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(descriptor, fromType.Locations.FirstOrDefault(), fromType.Name, namedToType.Name));
    }

    private static Layer GetLayer(string? namespaceName)
    {
        if (string.IsNullOrEmpty(namespaceName))
        {
            return Layer.Unknown;
        }

        if (namespaceName.Contains(".Api", System.StringComparison.Ordinal))
        {
            return Layer.Api;
        }

        if (namespaceName.Contains(".Application", System.StringComparison.Ordinal))
        {
            return Layer.Application;
        }

        if (namespaceName.Contains(".Domain", System.StringComparison.Ordinal))
        {
            return Layer.Domain;
        }

        if (namespaceName.Contains(".Infrastructure", System.StringComparison.Ordinal))
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