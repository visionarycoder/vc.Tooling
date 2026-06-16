using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Security.Rules;

internal sealed class SqlInjectionRule : IAnalyzerRule
{
    private static readonly ImmutableHashSet<string> SqlMethods = ImmutableHashSet.Create(
        System.StringComparer.OrdinalIgnoreCase,
        "CommandText", "ExecuteSqlRaw", "FromSqlRaw", "SqlQuery");

    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.SecuritySqlInjection,
        title: "Potential SQL injection",
        messageFormat: "SQL command text is built via string concatenation or interpolation with '{0}'. Use parameterized queries instead.",
        category: "Security",
        defaultSeverity: DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeAssignment, SyntaxKind.SimpleAssignmentExpression);
        context.RegisterSyntaxNodeAction(AnalyzeInvocation, SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;
        if (!assignment.Left.ToString().EndsWith("CommandText", System.StringComparison.Ordinal))
        {
            return;
        }

        if (assignment.Right is InterpolatedStringExpressionSyntax or BinaryExpressionSyntax)
        {
            var identifiers = assignment.Right.DescendantNodes().OfType<IdentifierNameSyntax>().Select(identifier => identifier.Identifier.Text);
            context.ReportDiagnostic(Diagnostic.Create(descriptor, assignment.Right.GetLocation(), string.Join(", ", identifiers)));
        }
    }

    private static void AnalyzeInvocation(SyntaxNodeAnalysisContext context)
    {
        var invocation = (InvocationExpressionSyntax)context.Node;
        var methodName = invocation.Expression switch
        {
            MemberAccessExpressionSyntax memberAccess => memberAccess.Name.Identifier.Text,
            _ => null
        };

        if (methodName is null || !SqlMethods.Contains(methodName))
        {
            return;
        }

        var arguments = invocation.ArgumentList.Arguments;
        if (arguments.Count > 0 && arguments[0].Expression is InterpolatedStringExpressionSyntax or BinaryExpressionSyntax)
        {
            var identifiers = arguments[0].Expression.DescendantNodes().OfType<IdentifierNameSyntax>().Select(identifier => identifier.Identifier.Text);
            context.ReportDiagnostic(Diagnostic.Create(descriptor, arguments[0].GetLocation(), string.Join(", ", identifiers)));
        }
    }
}