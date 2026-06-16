using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Security.Rules;

internal sealed class HardcodedSecretRule : IAnalyzerRule
{
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

    private static readonly Regex SecretPattern = new(
        @"(password|secret|apikey|api_key|token|connectionstring|conn_str)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.SecurityHardcodedSecret,
        title: "Hardcoded secret detected",
        messageFormat: "Potential hardcoded secret detected in '{0}'. Use IConfiguration or a secrets manager instead.",
        category: "Security",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
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

        var variableName = TryGetAssignedVariableName(literal);
        var matchesVariableHeuristic = !string.IsNullOrWhiteSpace(variableName) &&
            SecretPattern.IsMatch(variableName) &&
            value.Length >= 4;
        var matchesValueHeuristic = IsPotentialSecret(value);

        if (!matchesVariableHeuristic && !matchesValueHeuristic)
        {
            return;
        }

        var subject = matchesVariableHeuristic ? variableName! : Truncate(value, 32);
        context.ReportDiagnostic(Diagnostic.Create(descriptor, literal.GetLocation(), subject));
    }

    private static string? TryGetAssignedVariableName(LiteralExpressionSyntax literal)
    {
        if (literal.Parent is AssignmentExpressionSyntax assignment)
        {
            return assignment.Left.ToString();
        }

        if (literal.Parent is EqualsValueClauseSyntax equals && equals.Parent is VariableDeclaratorSyntax declarator)
        {
            return declarator.Identifier.Text;
        }

        return null;
    }

    private static bool IsPotentialSecret(string value)
    {
        if (value.Length < 12)
        {
            return false;
        }

        if (SuspiciousPrefixes.Any(prefix => value.StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        if (SuspiciousSubstrings.Any(sub => value.Contains(sub, System.StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        return LooksLikeBase64(value) || LooksHighEntropy(value);
    }

    private static bool LooksLikeBase64(string value)
    {
        if (value.Length % 4 != 0)
        {
            return false;
        }

        return value.All(character => char.IsLetterOrDigit(character) || character is '+' or '/' or '=');
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
        return value.Length <= max ? value : value.Substring(0, max) + "...";
    }
}