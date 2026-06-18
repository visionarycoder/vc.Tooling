using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Api.Rules;

internal sealed class NonRestfulNameRule : IAnalyzerRule
{
    private static readonly string[] HttpAttributes = ["HttpGet", "HttpPost", "HttpPut", "HttpPatch", "HttpDelete"];
    private static readonly string[] AllowedPrefixes = ["Get", "Post", "Put", "Patch", "Delete", "List", "Create", "Update", "Remove"
    ];

    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ApiDesignNaming,
        title: "Non-RESTful action name",
        messageFormat: "Action '{0}' may not follow RESTful naming conventions",
        category: "ApiDesign",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(action: AnalyzeMethod, symbolKinds: SymbolKind.Method);
    }

    private static void AnalyzeMethod(SymbolAnalysisContext context)
    {
        if (context.Symbol is not IMethodSymbol method || method.MethodKind != MethodKind.Ordinary)
        {
            return;
        }

        if (!HasAnyAttribute(symbol: method, names: HttpAttributes))
        {
            return;
        }

        if (AllowedPrefixes.Any(predicate: prefix => method.Name.StartsWith(value: prefix, comparisonType: System.StringComparison.Ordinal)))
        {
            return;
        }

        context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: method.Locations.FirstOrDefault(), messageArgs: method.Name));
    }

    private static bool HasAnyAttribute(ISymbol symbol, string[] names)
    {
        return symbol.GetAttributes().Any(predicate: attribute =>
        {
            var name = attribute.AttributeClass?.Name?.Replace(oldValue: "Attribute", newValue: string.Empty) ?? string.Empty;
            return names.Contains(value: name);
        });
    }
}