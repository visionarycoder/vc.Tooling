using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Api.Rules;

internal sealed class ApiResponseTypeRule : IAnalyzerRule
{
    private static readonly string[] HttpAttributes = { "HttpGet", "HttpPost", "HttpPut", "HttpPatch", "HttpDelete" };

    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.ApiResponseSerializationIssue,
        title: "API action missing response metadata",
        messageFormat: "Action '{0}' returns '{1}' but has no Produces/ProducesResponseType metadata.",
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

        if (HasAnyAttribute(method, new[] { "Produces", "ProducesResponseType" }))
        {
            return;
        }

        var returnType = method.ReturnType;
        var returnName = returnType.Name;

        if (returnName is "Void" or "IActionResult")
        {
            return;
        }

        if (returnName == "Task" && returnType is INamedTypeSymbol taskType && taskType.TypeArguments.Length == 1)
        {
            returnName = taskType.TypeArguments[0].Name;
        }

        context.ReportDiagnostic(Diagnostic.Create(descriptor, method.Locations.FirstOrDefault(), method.Name, returnName));
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

