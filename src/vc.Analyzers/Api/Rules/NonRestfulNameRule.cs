using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Api.Rules;

internal sealed class NonRestfulNameRule : IAnalyzerRule
{
    private static readonly string[] HttpAttributes = { "HttpGet", "HttpPost", "HttpPut", "HttpPatch", "HttpDelete" };
    private static readonly string[] AllowedPrefixes = { "Get", "Post", "Put", "Patch", "Delete", "List", "Create", "Update", "Remove" };

    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ApiDesignNaming,
        title: "Non-RESTful action name",
        messageFormat: "Action '{0}' may not follow RESTful naming conventions.",
        category: "ApiDesign",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyzeMethod, SymbolKind.Method);
    }

    private static void AnalyzeMethod(SymbolAnalysisContext context)
    {
        if (context.Symbol is not IMethodSymbol method || method.MethodKind != MethodKind.Ordinary)
        {
            return;
        }

        if (!HasAnyAttribute(method, HttpAttributes))
        {
            return;
        }

        if (AllowedPrefixes.Any(prefix => method.Name.StartsWith(prefix, System.StringComparison.Ordinal)))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(descriptor, method.Locations.FirstOrDefault(), method.Name));
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