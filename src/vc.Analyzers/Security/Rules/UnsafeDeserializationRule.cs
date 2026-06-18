using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Security.Rules;

internal sealed class UnsafeDeserializationRule : IAnalyzerRule
{
    private static readonly string[] DangerousTypes =
    [
        "BinaryFormatter",
        "SoapFormatter",
        "LosFormatter",
        "NetDataContractSerializer",
        "JavaScriptSerializer"
    ];

    private static readonly string[] DangerousMethods =
    [
        "Deserialize",
        "DeserializeAsync",
        "ReadObject",
        "ReadObjectAsync"
    ];

    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.SecurityUnsafeDeserialization,
        title: "Unsafe deserialization",
        messageFormat: "Deserialization method '{0}' may allow unsafe or untrusted input",
        category: "Security",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(action: AnalyzeInvocation, syntaxKinds: SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        var symbol = context.SemanticModel.GetSymbolInfo(expression: invocation, cancellationToken: context.CancellationToken).Symbol as IMethodSymbol;
        if (symbol is null)
        {
            return;
        }

        if (IsKnownDangerousType(typeSymbol: symbol.ContainingType) || IsKnownDangerousMethod(methodSymbol: symbol) || IsSuspiciousCustomDeserializer(methodSymbol: symbol))
        {
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: invocation.GetLocation(), messageArgs: symbol.Name));
        }
    }

    private static bool IsKnownDangerousType(INamedTypeSymbol typeSymbol)
    {
        return typeSymbol is not null && DangerousTypes.Contains(value: typeSymbol.Name);
    }

    private static bool IsKnownDangerousMethod(IMethodSymbol methodSymbol)
    {
        if (!DangerousMethods.Contains(value: methodSymbol.Name))
        {
            return false;
        }

        var typeName = methodSymbol.ContainingType?.Name;
        if (typeName is null)
        {
            return false;
        }

        return typeName.Contains(value: "JsonSerializer", comparisonType: System.StringComparison.OrdinalIgnoreCase) ||
               typeName.Contains(value: "XmlSerializer", comparisonType: System.StringComparison.OrdinalIgnoreCase);
    }

    private static bool IsSuspiciousCustomDeserializer(IMethodSymbol methodSymbol)
    {
        if (!methodSymbol.Name.Contains(value: "Deserialize", comparisonType: System.StringComparison.OrdinalIgnoreCase) || methodSymbol.Parameters.Length == 0)
        {
            return false;
        }

        var firstParameter = methodSymbol.Parameters[index: 0];
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