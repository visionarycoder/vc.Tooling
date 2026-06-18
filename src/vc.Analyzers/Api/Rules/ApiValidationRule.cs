using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Api.Rules;

internal sealed class ApiValidationRule : IAnalyzerRule
{
    private static readonly string[] ValidationAttributes =
    [
        "Required",
        "Range",
        "StringLength",
        "MinLength",
        "MaxLength",
        "RegularExpression"
    ];

    private static readonly string[] HttpAttributes =
    [
        "HttpGet",
        "HttpPost",
        "HttpPut",
        "HttpPatch",
        "HttpDelete"
    ];

    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ApiValidationMissing,
        title: "API action missing input validation",
        messageFormat: "Action '{0}' has body-like input but no validation attributes were found",
        category: "ApiDesign",
        defaultSeverity: DiagnosticSeverity.Warning,
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

        var candidateParameters = method.Parameters.Where(predicate: IsComplexInput).ToList();
        if (!candidateParameters.Any())
        {
            return;
        }

        var hasValidation = candidateParameters.Any(predicate: p =>
            p.GetAttributes().Any(predicate: a => ValidationAttributes.Contains(value: a.AttributeClass?.Name?.Replace(oldValue: "Attribute", newValue: string.Empty) ?? string.Empty)));

        if (!hasValidation)
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: method.Locations.FirstOrDefault(), messageArgs: method.Name));
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

    private static bool IsComplexInput(IParameterSymbol parameter)
    {
        if (parameter.Type.SpecialType != SpecialType.None)
        {
            return false;
        }

        if (parameter.Type.TypeKind == TypeKind.Enum)
        {
            return false;
        }

        var typeName = parameter.Type.Name;
        return !string.Equals(a: typeName, b: "String", comparisonType: System.StringComparison.Ordinal);
    }
}

