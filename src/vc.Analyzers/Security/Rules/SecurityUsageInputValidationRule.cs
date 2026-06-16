using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Security.Rules;

internal sealed class SecurityUsageInputValidationRule : IAnalyzerRule
{
    private static readonly string[] HttpAttributes = { "HttpGet", "HttpPost", "HttpPut", "HttpPatch", "HttpDelete" };
    private static readonly string[] ValidationAttributes = { "Required", "Range", "StringLength", "MinLength", "MaxLength", "RegularExpression" };

    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.SecurityInputValidationMissing,
        title: "Endpoint missing input validation",
        messageFormat: "Endpoint '{0}' accepts complex input without validation attributes.",
        category: "Security",
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

        var complexInputs = method.Parameters.Where(IsComplexInput).ToList();
        if (!complexInputs.Any())
        {
            return;
        }

        var hasValidation = complexInputs.Any(p => p.GetAttributes().Any(a =>
        {
            var name = a.AttributeClass?.Name?.Replace("Attribute", string.Empty) ?? string.Empty;
            return ValidationAttributes.Contains(name);
        }));

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

        return !string.Equals(parameter.Type.Name, "String", System.StringComparison.Ordinal);
    }
}

