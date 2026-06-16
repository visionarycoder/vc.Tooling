using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis.Operations;

namespace VisionaryCoder.Tooling.SecurityGenerators;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class SqlInjectionAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor Rule = new(
        id: "VC3002",
        title: "Potential SQL injection",
        messageFormat: "SQL command text is built via string concatenation or interpolation with '{0}'. Use parameterized queries instead.",
        category: "Security",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    private static readonly ImmutableHashSet<string> SqlMethods = ImmutableHashSet.Create(
        StringComparer.OrdinalIgnoreCase,
        "CommandText", "ExecuteSqlRaw", "FromSqlRaw", "SqlQuery");

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeAssignment, SyntaxKind.SimpleAssignmentExpression);
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeAssignment(SyntaxNodeAnalysisContext ctx)
    {
        var assignment = (AssignmentExpressionSyntax)ctx.Node;
        var target = assignment.Left.ToString();
        if (!target.EndsWith("CommandText")) return;

        if (assignment.Right is InterpolatedStringExpressionSyntax or BinaryExpressionSyntax)
        {
            var identifiers = assignment.Right.DescendantNodes().OfType<IdentifierNameSyntax>().Select(i => i.Identifier.Text);
            ctx.ReportDiagnostic(Diagnostic.Create(Rule, assignment.Right.GetLocation(), string.Join(", ", identifiers)));
        }
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext ctx)
    {
        var invocation = (InvocationExpressionSyntax)ctx.Node;
        var methodName = invocation.Expression switch
        {
            MemberAccessExpressionSyntax m => m.Name.Identifier.Text,
            _ => null
        };
        if (methodName is null || !SqlMethods.Contains(methodName)) return;

        var args = invocation.ArgumentList.Arguments;
        if (args.Count > 0 && args[0].Expression is InterpolatedStringExpressionSyntax or BinaryExpressionSyntax)
        {
            var identifiers = args[0].Expression.DescendantNodes().OfType<IdentifierNameSyntax>().Select(i => i.Identifier.Text);
            ctx.ReportDiagnostic(Diagnostic.Create(Rule, args[0].GetLocation(), string.Join(", ", identifiers)));
        }
    }
}