namespace Vc.Analyzers.Security;

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
        "AKIA",
        "ASIA",
        "SK",
        "RK",
        "xoxp-",
        "xoxb-",
        "ghp_",
        "gho_",
        "AIza",
        "EAAC",
        "ssh-rsa",
        "ssh-ed25519"
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
