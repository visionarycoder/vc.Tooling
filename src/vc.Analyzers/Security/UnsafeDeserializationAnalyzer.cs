namespace Vc.Analyzers.Security;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class UnsafeDeserializationAnalyzer : DiagnosticAnalyzer
{
    public const string UnsafeDeserializationId = "VCSEC003";

    private static readonly DiagnosticDescriptor UnsafeDeserializationRule = new(
        UnsafeDeserializationId,
        "Unsafe deserialization",
        "Deserialization method '{0}' may allow unsafe or untrusted input.",
        "Security",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

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

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(UnsafeDeserializationRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not InvocationExpressionSyntax invocation)
        {
            return;
        }

        var semanticModel = context.SemanticModel;
        var cancellationToken = context.CancellationToken;

        var symbol = semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol as IMethodSymbol;
        if (symbol is null)
        {
            return;
        }

        if (IsKnownDangerousType(symbol.ContainingType))
        {
            Report(context, invocation, symbol);
            return;
        }

        if (IsKnownDangerousMethod(symbol))
        {
            Report(context, invocation, symbol);
            return;
        }

        if (IsSuspiciousCustomDeserializer(symbol))
        {
            Report(context, invocation, symbol);
        }
    }

    private static bool IsKnownDangerousType(INamedTypeSymbol typeSymbol)
    {
        if (typeSymbol is null)
        {
            return false;
        }

        var name = typeSymbol.Name;

        return DangerousTypes.Contains(name);
    }

    private static bool IsKnownDangerousMethod(IMethodSymbol methodSymbol)
    {
        var name = methodSymbol.Name;

        if (!DangerousMethods.Contains(name))
        {
            return false;
        }

        var typeName = methodSymbol.ContainingType?.Name;
        if (typeName is null)
        {
            return false;
        }

        if (typeName.Contains("JsonSerializer", System.StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        if (typeName.Contains("XmlSerializer", System.StringComparison.OrdinalIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private static bool IsSuspiciousCustomDeserializer(IMethodSymbol methodSymbol)
    {
        if (!methodSymbol.Name.Contains("Deserialize", System.StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (methodSymbol.Parameters.Length == 0)
        {
            return false;
        }

        var firstParam = methodSymbol.Parameters[0];

        if (firstParam.Type.SpecialType is SpecialType.System_String or SpecialType.System_Object)
        {
            return true;
        }

        if (firstParam.Type.Name is "Stream" or "MemoryStream")
        {
            return true;
        }

        if (firstParam.Type.Name is "ReadOnlySpan" or "Span" or "ReadOnlyMemory" or "Memory")
        {
            return true;
        }

        return false;
    }

    private static void Report(SyntaxNodeAnalysisContext context, InvocationExpressionSyntax invocation, IMethodSymbol methodSymbol)
    {
        context.ReportDiagnostic(
            Diagnostic.Create(
                UnsafeDeserializationRule,
                invocation.GetLocation(),
                methodSymbol.Name));
    }
}
