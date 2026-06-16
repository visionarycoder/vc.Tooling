using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Api.Rules;

internal sealed class ApiValidationRule : IAnalyzerRule
{
    private static readonly string[] ValidationAttributes =
    {
        "Required",
        "Range",
        "StringLength",
        "MinLength",
        "MaxLength",
        "RegularExpression"
    };

    private static readonly string[] HttpAttributes =
    {
        "HttpGet",
        "HttpPost",
        "HttpPut",
        "HttpPatch",
        "HttpDelete"
    };

    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ApiValidationMissing,
        title: "API action missing input validation",
        messageFormat: "Action '{0}' has body-like input but no validation attributes were found.",
        category: "ApiDesign",
        defaultSeverity: DiagnosticSeverity.Warning,
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

        var candidateParameters = method.Parameters.Where(IsComplexInput).ToList();
        if (!candidateParameters.Any())
        {
            return;
        }

        var hasValidation = candidateParameters.Any(p =>
            p.GetAttributes().Any(a => ValidationAttributes.Contains(a.AttributeClass?.Name?.Replace("Attribute", string.Empty) ?? string.Empty)));

        if (!hasValidation)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, method.Locations.FirstOrDefault(), method.Name));
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
        return !string.Equals(typeName, "String", System.StringComparison.Ordinal);
    }
}

