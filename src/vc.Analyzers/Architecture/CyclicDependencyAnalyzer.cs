using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Vc.Analyzers.Architecture;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class CyclicDependencyAnalyzer : DiagnosticAnalyzer
{
    public const string CyclicDependencyId = "VCARCH002";

    private static readonly DiagnosticDescriptor CyclicDependencyRule = new(
        CyclicDependencyId,
        "Cyclic namespace dependency detected",
        "Namespace '{0}' depends on '{1}', forming a cyclic dependency.",
        "Architecture",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(CyclicDependencyRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(Start);
    }

    private static void Start(CompilationStartAnalysisContext context)
    {
        var graph = new DependencyGraph();

        context.RegisterSymbolAction(symbolContext =>
        {
            if (symbolContext.Symbol is not INamedTypeSymbol typeSymbol)
            {
                return;
            }

            var fromNamespace = typeSymbol.ContainingNamespace?.ToDisplayString();
            if (string.IsNullOrEmpty(fromNamespace))
            {
                return;
            }

            foreach (var referenced in GetReferencedNamespaces(typeSymbol))
            {
                if (referenced == fromNamespace)
                {
                    continue;
                }

                graph.AddEdge(fromNamespace, referenced, typeSymbol);
            }
        }, SymbolKind.NamedType);

        context.RegisterCompilationEndAction(endContext =>
        {
            foreach (var cycle in graph.FindCycles())
            {
                var (from, to, typeSymbol) = cycle;

                endContext.ReportDiagnostic(
                    Diagnostic.Create(
                        CyclicDependencyRule,
                        typeSymbol.Locations.FirstOrDefault(),
                        from,
                        to));
            }
        });
    }

    private static IEnumerable<string> GetReferencedNamespaces(INamedTypeSymbol typeSymbol)
    {
        foreach (var member in typeSymbol.GetMembers())
        {
            switch (member)
            {
                case IMethodSymbol method:
                    if (method.ReturnType?.ContainingNamespace is { } retNs)
                    {
                        yield return retNs.ToDisplayString();
                    }

                    foreach (var p in method.Parameters)
                    {
                        if (p.Type.ContainingNamespace is { } pNs)
                        {
                            yield return pNs.ToDisplayString();
                        }
                    }

                    break;

                case IPropertySymbol property:
                    if (property.Type.ContainingNamespace is { } propNs)
                    {
                        yield return propNs.ToDisplayString();
                    }

                    break;

                case IFieldSymbol field:
                    if (field.Type.ContainingNamespace is { } fieldNs)
                    {
                        yield return fieldNs.ToDisplayString();
                    }

                    break;
            }
        }
    }

}