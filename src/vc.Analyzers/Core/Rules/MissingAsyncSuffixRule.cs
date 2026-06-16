using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Core.Rules;

internal sealed class MissingAsyncSuffixRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.AsyncMissingNameSuffix,
        "Async method should end with 'Async'",
        "Async method '{0}' should be suffixed with 'Async'.",
        "AsyncCorrectness",
        DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
    }

    private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
    {
        var method = (MethodDeclarationSyntax)context.Node;
        if (method.Modifiers.Any(SyntaxKind.AsyncKeyword) && !method.Identifier.Text.EndsWith("Async", StringComparison.Ordinal))
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, method.Identifier.GetLocation(), method.Identifier.Text));
        }
    }
}