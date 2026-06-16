using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vc.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class LayeringAnalyzer : DiagnosticAnalyzer
{
    public const string LayerViolationId = "VCARCH001";

    private static readonly DiagnosticDescriptor LayerViolationRule = new(
        LayerViolationId,
        "Layering rule violation",
        "Type '{0}' references '{1}', violating layering rules.",
        "Architecture",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(LayerViolationRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // We use a compilation action so we can see all types and their references.
        context.RegisterCompilationStartAction(StartCompilation);
    }

    private static void StartCompilation(CompilationStartAnalysisContext context)
    {
        // NOTE: This is intentionally simple and attribute-driven so future agents
        // can plug in richer configuration (additional files, JSON, etc.).
        //
        // Convention:
        // - Layers are defined via [Layer("Name")] on namespaces or assemblies.
        // - Dependencies are allowed only from lower to higher layers (e.g., Api -> Application -> Domain).
        //
        // For Pass 2, we implement a minimal version:
        // - Infer layer from namespace segments: *.Api, *.Application, *.Domain, *.Infrastructure.
        // - Enforce: Api can depend on Application; Application can depend on Domain; Domain cannot depend on Application or Api; Infrastructure cannot be referenced by Api or Application directly.

        context.RegisterSymbolAction(AnalyzeNamedType, SymbolKind.NamedType);
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
            AnalyzeMemberDependencies(context, typeSymbol, member, fromLayer);
        }
    }

    private static void AnalyzeMemberDependencies(SymbolAnalysisContext context, INamedTypeSymbol fromType, ISymbol member, Layer fromLayer)
    {
        switch (member)
        {
            case IMethodSymbol method:
                AnalyzeMethodDependencies(context, fromType, method, fromLayer);
                break;
            case IPropertySymbol property:
                AnalyzeTypeDependency(context, fromType, property.Type, fromLayer);
                break;
            case IFieldSymbol field:
                AnalyzeTypeDependency(context, fromType, field.Type, fromLayer);
                break;
        }
    }

    private static void AnalyzeMethodDependencies(SymbolAnalysisContext context, INamedTypeSymbol fromType, IMethodSymbol method, Layer fromLayer)
    {
        AnalyzeTypeDependency(context, fromType, method.ReturnType, fromLayer);

        foreach (var parameter in method.Parameters)
        {
            AnalyzeTypeDependency(context, fromType, parameter.Type, fromLayer);
        }
    }

    private static void AnalyzeTypeDependency(SymbolAnalysisContext context, INamedTypeSymbol fromType, ITypeSymbol toType, Layer fromLayer)
    {
        if (toType is null)
        {
            return;
        }

        var namedToType = toType as INamedTypeSymbol ?? toType.OriginalDefinition as INamedTypeSymbol;
        if (namedToType is null)
        {
            return;
        }

        var toNamespace = namedToType.ContainingNamespace?.ToDisplayString();
        var toLayer = GetLayer(toNamespace);

        if (toLayer == Layer.Unknown)
        {
            return;
        }

        if (!IsAllowedDependency(fromLayer, toLayer))
        {
            context.ReportDiagnostic(
                Diagnostic.Create(
                    LayerViolationRule,
                    fromType.Locations.FirstOrDefault(),
                    fromType.Name,
                    namedToType.Name));
        }
    }

    private static Layer GetLayer(string? namespaceName)
    {
        if (string.IsNullOrEmpty(namespaceName))
        {
            return Layer.Unknown;
        }

        if (namespaceName.Contains(".Api"))
        {
            return Layer.Api;
        }

        if (namespaceName.Contains(".Application"))
        {
            return Layer.Application;
        }

        if (namespaceName.Contains(".Domain"))
        {
            return Layer.Domain;
        }

        if (namespaceName.Contains(".Infrastructure"))
        {
            return Layer.Infrastructure;
        }

        return Layer.Unknown;
    }

    private static bool IsAllowedDependency(Layer from, Layer to)
    {
        // Simple rule set:
        // - Api -> Application, Domain (allowed)
        // - Application -> Domain (allowed)
        // - Domain -> Domain only (no upward references)
        // - Infrastructure -> Domain (allowed), but not referenced by Api/Application directly

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
        Unknown = 0,
        Api,
        Application,
        Domain,
        Infrastructure
    }
}
