using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Design.Vbd.Rules;

internal sealed class VbdManagerOrchestrationRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        id: DiagnosticIds.VbdManagerVaultMissingOrchestration,
        title: "Manager missing orchestration",
        messageFormat: "Manager type '{0}' has no orchestration-style method calls.",
        category: "VbdManager",
        defaultSeverity: DiagnosticSeverity.Info,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
        context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
    }

    private static void AnalyzeType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol type || type.TypeKind != TypeKind.Class)
        {
            return;
        }

        if (!type.Name.EndsWith("Manager"))
        {
            return;
        }

        var methods = type.GetMembers().OfType<IMethodSymbol>()
            .Where(m => m.MethodKind == MethodKind.Ordinary && m.DeclaringSyntaxReferences.Length > 0)
            .ToList();

        if (!methods.Any())
        {
            return;
        }

        var hasOrchestrationCall = false;
        foreach (var method in methods)
        {
            var syntax = method.DeclaringSyntaxReferences[0].GetSyntax() as MethodDeclarationSyntax;
            if (syntax == null)
            {
                continue;
            }

            if (syntax.DescendantNodes().OfType<InvocationExpressionSyntax>().Any())
            {
                hasOrchestrationCall = true;
                break;
            }
        }

        if (!hasOrchestrationCall)
        {
            context.ReportDiagnostic(Diagnostic.Create(descriptor, type.Locations.FirstOrDefault(), type.Name));
        }
    }
}

