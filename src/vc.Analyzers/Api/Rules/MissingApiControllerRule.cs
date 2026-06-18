using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Api.Rules;

internal sealed class MissingApiControllerRule : IAnalyzerRule
{
    private static readonly string[] HttpAttributes = ["HttpGet", "HttpPost", "HttpPut", "HttpPatch", "HttpDelete"];

    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ApiDesignControllerMissing,
        title: "API controller should be annotated",
        messageFormat: "Public controller '{0}' should be annotated with [ApiController]",
        category: "ApiDesign",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(action: AnalyzeType, symbolKinds: SymbolKind.NamedType);
    }

    private static void AnalyzeType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol type || type.TypeKind != TypeKind.Class)
        {
            return;
        }

        if (!type.DeclaredAccessibility.HasFlag(flag: Accessibility.Public))
        {
            return;
        }

        if (!LooksLikeController(type: type))
        {
            return;
        }

        if (HasAnyAttribute(symbol: type, names: ["ApiController"]))
        {
            return;
        }

        context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: type.Locations.FirstOrDefault(), messageArgs: type.Name));
    }

    private static bool LooksLikeController(INamedTypeSymbol type)
    {
        if (type.Name.EndsWith(value: "Controller", comparisonType: System.StringComparison.Ordinal))
        {
            return true;
        }

        if (type.BaseType?.Name is "ControllerBase" or "Controller")
        {
            return true;
        }

        return type.GetMembers().OfType<IMethodSymbol>()
            .Any(predicate: member => member.MethodKind == MethodKind.Ordinary && HasAnyAttribute(symbol: member, names: HttpAttributes));
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