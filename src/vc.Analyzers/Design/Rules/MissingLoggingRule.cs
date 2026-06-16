using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Rules;

internal sealed class MissingLoggingRule : IAnalyzerRule
{
    private static readonly string[] LoggingMethodHints = { "Log", "Trace", "Error", "Warn", "Critical", "TrackException" };

    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        "VCDESIGN002",
        "Missing logging in catch block",
        "Exception caught but not logged. Catch blocks should log, wrap, or rethrow exceptions.",
        "Design",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeCatchClause, SyntaxKind.CatchClause);
    }

    private static void AnalyzeCatchClause(SyntaxNodeAnalysisContext context)
    {
        if (context.Node is not CatchClauseSyntax catchClause || catchClause.Block is null)
        {
            return;
        }

        if (ContainsRethrow(catchClause.Block) || ContainsLogging(context.SemanticModel, catchClause.Block, context.CancellationToken))
        {
            return;
        }

        context.ReportDiagnostic(Diagnostic.Create(descriptor, catchClause.CatchKeyword.GetLocation()));
    }

    private static bool ContainsRethrow(BlockSyntax block)
    {
        return block.DescendantNodes().OfType<ThrowStatementSyntax>().Any(throwStatement => throwStatement.Expression is null);
    }

    private static bool ContainsLogging(SemanticModel semanticModel, BlockSyntax block, System.Threading.CancellationToken cancellationToken)
    {
        foreach (var invocation in block.DescendantNodes().OfType<InvocationExpressionSyntax>())
        {
            var symbol = semanticModel.GetSymbolInfo(invocation, cancellationToken).Symbol as IMethodSymbol;
            if (symbol is null)
            {
                continue;
            }

            if (LoggingMethodHints.Any(hint => symbol.Name.Contains(hint, StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }

            var containingType = symbol.ContainingType?.Name;
            if (containingType is not null && (containingType.Contains("Logger", StringComparison.OrdinalIgnoreCase) || containingType.Contains("Telemetry", StringComparison.OrdinalIgnoreCase)))
            {
                return true;
            }
        }

        return false;
    }
}