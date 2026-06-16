using System.Collections.Immutable;
using System.Text.RegularExpressions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VisionaryCoder.Tooling.SecurityGenerators;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class HardcodedSecretAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor Rule = new(
        id: "VC3001",
        title: "Hardcoded secret detected",
        messageFormat: "String literal assigned to '{0}' appears to contain a secret. Use IConfiguration or a secrets manager instead.",
        category: "Security",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly Regex SecretPattern = new(
        @"(password|secret|apikey|api_key|token|connectionstring|conn_str)",
        RegexOptions.IgnoreCase | RegexOptions.Compiled);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(Analyze, SyntaxKind.SimpleAssignmentExpression, SyntaxKind.EqualsValueClause);
    }

    private static void Analyze(SyntaxNodeAnalysisContext ctx)
    {
        string? variableName = null;
        ExpressionSyntax? value = null;

        if (ctx.Node is AssignmentExpressionSyntax assignment)
        {
            variableName = assignment.Left.ToString();
            value = assignment.Right;
        }
        else if (ctx.Node is EqualsValueClauseSyntax equals && equals.Parent is VariableDeclaratorSyntax declarator)
        {
            variableName = declarator.Identifier.Text;
            value = equals.Value;
        }

        if (variableName is null || value is not LiteralExpressionSyntax literal) return;
        if (literal.Kind() != SyntaxKind.StringLiteralExpression) return;
        if (!SecretPattern.IsMatch(variableName)) return;
        if (literal.Token.ValueText.Length < 4) return;

        ctx.ReportDiagnostic(Diagnostic.Create(Rule, literal.GetLocation(), variableName));
    }
}