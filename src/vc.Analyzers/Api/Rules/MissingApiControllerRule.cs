using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Api.Rules;

internal sealed class MissingApiControllerRule : IAnalyzerRule
{
    private static readonly string[] HttpAttributes = { "HttpGet", "HttpPost", "HttpPut", "HttpPatch", "HttpDelete" };

    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ApiDesignControllerMissing,
        title: "API controller should be annotated",
        messageFormat: "Public controller '{0}' should be annotated with [ApiController].",
        category: "ApiDesign",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
    }

    private static void AnalyzeType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol type || type.TypeKind != TypeKind.Class)
        {
            return;
        }

        if (!type.DeclaredAccessibility.HasFlag(Accessibility.Public))
        {
            return;
        }

        if (!LooksLikeController(type))
        {
            return;
        }

        if (HasAnyAttribute(type, new[] { "ApiController" }))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(descriptor, type.Locations.FirstOrDefault(), type.Name));
    }

    private static bool LooksLikeController(INamedTypeSymbol type)
    {
        if (type.Name.EndsWith("Controller", System.StringComparison.Ordinal))
        {
            return true;
        }

        if (type.BaseType?.Name is "ControllerBase" or "Controller")
        {
            return true;
        }

        return type.GetMembers().OfType<IMethodSymbol>()
            .Any(member => member.MethodKind == MethodKind.Ordinary && HasAnyAttribute(member, HttpAttributes));
    }

    private static bool HasAnyAttribute(ISymbol symbol, string[] names)
    {
        return symbol.GetAttributes().Any(attribute =>
        {
            var name = attribute.AttributeClass?.Name?.Replace("Attribute", string.Empty) ?? string.Empty;
            return names.Contains(name);
        });
    }
}