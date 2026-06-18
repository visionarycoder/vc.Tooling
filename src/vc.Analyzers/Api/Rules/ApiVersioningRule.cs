using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Api.Rules;

internal sealed class ApiVersioningRule : IAnalyzerRule
{
    private static readonly string[] HttpAttributes = ["HttpGet", "HttpPost", "HttpPut", "HttpPatch", "HttpDelete"];

    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ApiVersioningMissing,
        title: "API controller missing versioning",
        messageFormat: "Controller '{0}' exposes HTTP actions but has no API version attribute",
        category: "ApiDesign",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(action: AnalyzeController, symbolKinds: SymbolKind.NamedType);
    }

    private static void AnalyzeController(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol type || type.TypeKind != TypeKind.Class)
        {
            return;
        }

        if (!type.Name.EndsWith(value: "Controller", comparisonType: System.StringComparison.Ordinal))
        {
            return;
        }

        var actions = type.GetMembers().OfType<IMethodSymbol>()
            .Where(predicate: m => m.MethodKind == MethodKind.Ordinary && HasAnyAttribute(symbol: m, names: HttpAttributes))
            .ToList();

        if (!actions.Any())
        {
            return;
        }

        var hasVersionAttribute = HasAnyAttribute(symbol: type, names: ["ApiVersion", "ApiVersionNeutral"]) ||
                                  actions.Any(predicate: m => HasAnyAttribute(symbol: m, names: ["ApiVersion", "MapToApiVersion"]));

        if (!hasVersionAttribute)
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: type.Locations.FirstOrDefault(), messageArgs: type.Name));
        }
    }

    private static bool HasAnyAttribute(ISymbol symbol, string[] names)
    {
        return symbol.GetAttributes().Any(predicate: a =>
        {
            var name = a.AttributeClass?.Name?.Replace(oldValue: "Attribute", newValue: string.Empty) ?? string.Empty;
            return names.Contains(value: name);
        });
    }
}

