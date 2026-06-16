using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Security.Rules;

internal sealed class UnsafeDeserializationRule : IAnalyzerRule
{
    private static readonly string[] DangerousTypes =
    {
        "BinaryFormatter",
        "SoapFormatter",
        "LosFormatter",
        "NetDataContractSerializer",
        "JavaScriptSerializer"
    };

    private static readonly string[] DangerousMethods =
    {
        "Deserialize",
        "DeserializeAsync",
        "ReadObject",
        "ReadObjectAsync"
    };

    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.SecurityUnsafeDeserialization,
        title: "Unsafe deserialization",
        messageFormat: "Deserialization method '{0}' may allow unsafe or untrusted input.",
        category: "Security",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        var symbol = context.SemanticModel.GetSymbolInfo(invocation, context.CancellationToken).Symbol as IMethodSymbol;
        if (symbol is null)
        {
            return;
        }

        if (IsKnownDangerousType(symbol.ContainingType) || IsKnownDangerousMethod(symbol) || IsSuspiciousCustomDeserializer(symbol))
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, invocation.GetLocation(), symbol.Name));
        }
    }

    private static bool IsKnownDangerousType(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol is not null && DangerousTypes.Contains(typeSymbol.Name);
    }

    private static bool IsKnownDangerousMethod(IMethodSymbol methodSymbol)
    {
        if (!DangerousMethods.Contains(methodSymbol.Name))
        {
            return false;
        }

        var typeName = methodSymbol.ContainingType?.Name;
        if (typeName is null)
        {
            return false;
        }

        return typeName.Contains("JsonSerializer", System.StringComparison.OrdinalIgnoreCase) ||
               typeName.Contains("XmlSerializer", System.StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSuspiciousCustomDeserializer(IMethodSymbol methodSymbol)
    {
        if (!methodSymbol.Name.Contains("Deserialize", System.StringComparison.OrdinalIgnoreCase) || methodSymbol.Parameters.Length == 0)
        {
            return false;
        }

        var firstParameter = methodSymbol.Parameters[0];
        if (firstParameter.Type.SpecialType is SpecialType.System_String or SpecialType.System_Object)
        {
            return true;
        }

        if (firstParameter.Type.Name is "Stream" or "MemoryStream")
        {
            return true;
        }

        return firstParameter.Type.Name is "ReadOnlySpan" or "Span" or "ReadOnlyMemory" or "Memory";
    }
}