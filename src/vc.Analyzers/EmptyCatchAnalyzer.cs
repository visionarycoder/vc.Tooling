using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace VisionaryCoder.Tooling.Analyzers;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class EmptyCatchAnalyzer : DiagnosticAnalyzer
{
    public static readonly DiagnosticDescriptor EmptyRule = new(
        id: "VC2001",
        title: "Empty catch block",
        messageFormat: "Catch block is empty. At minimum, log the exception or add a justification comment.",
        category: "Reliability",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public static readonly DiagnosticDescriptor BroadRule = new(
        id: "VC2002",
        title: "Broad catch without logging",
        messageFormat: "Catching '{0}' without logging or re-throwing. Add logging or narrow the exception type.",
        category: "Reliability",
        defaultSeverity: DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(EmptyRule, BroadRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();
        context.RegisterSyntaxNodeAction(AnalyzeCatch, SyntaxKind.CatchClause);
    }

    private static void AnalyzeCatch(SyntaxNodeAnalysisContext ctx)
    {
        var catchClause = (CatchClauseSyntax)ctx.Node;
        var block = catchClause.Block;

        if (block.Statements.Count == 0)
        {
            ctx.ReportDiagnostic(Diagnostic.Create(EmptyRule, catchClause.CatchKeyword.GetLocation()));
            return;
        }

        var exceptionType = catchClause.Declaration?.Type?.ToString() ?? "Exception";
        if (exceptionType is "Exception" or "System.Exception")
        {
            var blockText = block.ToString();
            if (!blockText.Contains("log", StringComparison.OrdinalIgnoreCase) &&
                !blockText.Contains("throw"))
            {
                ctx.ReportDiagnostic(Diagnostic.Create(BroadRule, catchClause.CatchKeyword.GetLocation(), exceptionType));
            }
        }
    }
}