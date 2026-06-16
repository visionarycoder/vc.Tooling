using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Api.Rules;

internal sealed class ApiVersioningRule : IAnalyzerRule
{
    private static readonly string[] HttpAttributes = { "HttpGet", "HttpPost", "HttpPut", "HttpPatch", "HttpDelete" };

    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ApiVersioningMissing,
        title: "API controller missing versioning",
        messageFormat: "Controller '{0}' exposes HTTP actions but has no API version attribute.",
        category: "ApiDesign",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyzeController, SymbolKind.NamedType);
    }

    private static void AnalyzeController(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol type || type.TypeKind != TypeKind.Class)
        {
            return;
        }

        if (!type.Name.EndsWith("Controller", System.StringComparison.Ordinal))
        {
            return;
        }

        var actions = type.GetMembers().OfType<IMethodSymbol>()
            .Where(m => m.MethodKind == MethodKind.Ordinary && HasAnyAttribute(m, HttpAttributes))
            .ToList();

        if (!actions.Any())
        {
            return;
        }

        var hasVersionAttribute = HasAnyAttribute(type, new[] { "ApiVersion", "ApiVersionNeutral" }) ||
                                  actions.Any(m => HasAnyAttribute(m, new[] { "ApiVersion", "MapToApiVersion" }));

        if (!hasVersionAttribute)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, type.Locations.FirstOrDefault(), type.Name));
        }
    }

    private static bool HasAnyAttribute(ISymbol symbol, string[] names)
    {
        return symbol.GetAttributes().Any(a =>
        {
            var name = a.AttributeClass?.Name?.Replace("Attribute", string.Empty) ?? string.Empty;
            return names.Contains(name);
        });
    }
}

