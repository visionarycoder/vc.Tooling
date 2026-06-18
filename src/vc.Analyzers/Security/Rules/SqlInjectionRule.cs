using VisionaryCoder.Analyzers.Abstractions;

namespace VisionaryCoder.Analyzers.Security.Rules;

internal sealed class SqlInjectionRule : IAnalyzerRule
{
    private static readonly ImmutableHashSet<string> SqlMethods = ImmutableHashSet.Create(
        equalityComparer: System.StringComparer.OrdinalIgnoreCase,
        items: new[]{"CommandText", "ExecuteSqlRaw", "FromSqlRaw", "SqlQuery"});

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
        context.RegisterSyntaxNodeAction(action: AnalyzeAssignment, syntaxKinds: SyntaxKind.SimpleAssignmentExpression);
        context.RegisterSyntaxNodeAction(action: AnalyzeInvocation, syntaxKinds: SyntaxKind.InvocationExpression);
    }

    private static void AnalyzeAssignment(SyntaxNodeAnalysisContext context)
    {
        var assignment = (AssignmentExpressionSyntax)context.Node;
        if (!assignment.Left.ToString().EndsWith(value: "CommandText", comparisonType: System.StringComparison.Ordinal))
        {
            return;
        }

        if (assignment.Right is InterpolatedStringExpressionSyntax or BinaryExpressionSyntax)
        {
            var identifiers = assignment.Right.DescendantNodes().OfType<IdentifierNameSyntax>().Select(selector: identifier => identifier.Identifier.Text);
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: assignment.Right.GetLocation(), messageArgs: string.Join(separator: ", ", values: identifiers)));
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

        if (methodName is null || !SqlMethods.Contains(item: methodName))
        {
            return;
        }

        var arguments = invocation.ArgumentList.Arguments;
        if (arguments.Count > 0 && arguments[index: 0].Expression is InterpolatedStringExpressionSyntax or BinaryExpressionSyntax)
        {
            var identifiers = arguments[index: 0].Expression.DescendantNodes().OfType<IdentifierNameSyntax>().Select(selector: identifier => identifier.Identifier.Text);
            context.ReportDiagnostic(diagnostic: Diagnostic.Create(descriptor: descriptor, location: arguments[index: 0].GetLocation(), messageArgs: string.Join(separator: ", ", values: identifiers)));
        }
    }
}