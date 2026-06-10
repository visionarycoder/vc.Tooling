namespace Vc.Analyzers.Security;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SecurityUsageAnalyzer : DiagnosticAnalyzer
{
    public const string MissingSecureAttributeId = "VCSEC001";

    private static readonly DiagnosticDescriptor MissingSecureAttributeRule = new(
        id: MissingSecureAttributeId,
        title: "Public API should be secured",
        messageFormat: "Public method '{0}' should be annotated with [Secure] or equivalent security attribute.",
        category: "Security",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(MissingSecureAttributeRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        // TODO:
        // - Register symbol action for public methods in API assemblies.
        // - Detect presence of [Secure] or other security attributes.
        // - Report diagnostics when public endpoints are not secured.
    }
}

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class HardcodedSecretsAnalyzer : DiagnosticAnalyzer
{
    public const string HardcodedSecretId = "VCSEC002";

    private static readonly DiagnosticDescriptor HardcodedSecretRule = new(
        HardcodedSecretId,
        "Hardcoded secret detected",
        "Potential secret or credential detected in literal: '{0}'.",
        "Security",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    private static readonly string[] SuspiciousPrefixes =
    {
        "AKIA",         // AWS Access Key
        "ASIA",         // AWS Temporary Key
        "SK",           // Stripe Key
        "RK",           // Redis Key
        "xoxp-",        // Slack Token
        "xoxb-",        // Slack Bot Token
        "ghp_",         // GitHub Personal Access Token
        "gho_",         // GitHub OAuth Token
        "AIza",         // Google API Key
        "EAAC",         // Facebook Token
        "ssh-rsa",      // SSH Key
        "ssh-ed25519"   // SSH Key
    };

    private static readonly string[] SuspiciousSubstrings =
    {
        "token",
        "secret",
        "password",
        "apikey",
        "api_key",
        "connectionstring",
        "connstring",
        "privatekey",
        "clientsecret",
        "jwt",
        "bearer"
    };

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(HardcodedSecretRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSyntaxNodeAction(AnalyzeLiteral, SyntaxKind.StringLiteralExpression);
    }

    private static void AnalyzeLiteral(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not LiteralExpressionSyntax literal)
        {
            return;
        }

        var value = literal.Token.ValueText;
        if (string.IsNullOrWhiteSpace(value))
        {
            return;
        }

        if (!IsPotentialSecret(value))
        {
            return;
        }

        context.ReportDiagnostic(
            Diagnostic.Create(
                HardcodedSecretRule,
                literal.GetLocation(),
                Truncate(value, 32)));
    }

    private static bool IsPotentialSecret(string value)
    {
        if (value.Length < 12)
        {
            return false;
        }

        if (SuspiciousPrefixes.Any(prefix => value.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        if (SuspiciousSubstrings.Any(sub => value.Contains(sub, StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        if (LooksLikeBase64(value))
        {
            return true;
        }

        if (LooksHighEntropy(value))
        {
            return true;
        }

        return false;
    }

    private static bool LooksLikeBase64(string value)
    {
        if (value.Length % 4 != 0)
        {
            return false;
        }

        return value.All(c =>
            char.IsLetterOrDigit(c) ||
            c is '+' or '/' or '=');
    }

    private static bool LooksHighEntropy(string value)
    {
        if (value.Length < 20)
        {
            return false;
        }

        var uniqueChars = value.Distinct().Count();
        var ratio = (double)uniqueChars / value.Length;

        return ratio > 0.6;
    }

    private static string Truncate(string value, int max)
    {
        if (value.Length <= max)
        {
            return value;
        }

        return value.Substring(0, max) + "...";
    }
}

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
