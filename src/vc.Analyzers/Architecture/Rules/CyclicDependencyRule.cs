using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;
using VisionaryCoder.Tooling.Analyzers.Common;

namespace Vc.Analyzers.Architecture.Rules;

internal sealed class CyclicDependencyRule : IAnalyzerRule
{
    public DiagnosticDescriptor Descriptor => descriptor;

    private static readonly DiagnosticDescriptor descriptor = new(
        DiagnosticIds.ArchCyclicDependency,
        "Cyclic namespace dependency detected",
        "Namespace '{0}' depends on '{1}', forming a cyclic dependency.",
        "Architecture",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public void Register(AnalysisContext context)
    {
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
                if (referenced != fromNamespace)
                {
                    graph.AddEdge(fromNamespace, referenced, typeSymbol);
                }
            }
        }, SymbolKind.NamedType);

        context.RegisterCompilationEndAction(endContext =>
        {
            foreach (var cycle in graph.FindCycles())
            {
                var (from, to, typeSymbol) = cycle;
                endContext.ReportDiagnostic(Diagnostic.Create(descriptor, typeSymbol.Locations.FirstOrDefault(), from, to));
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
                    if (method.ReturnType?.ContainingNamespace is { } returnNamespace)
                    {
                        yield return returnNamespace.ToDisplayString();
                    }

                    foreach (var parameter in method.Parameters)
                    {
                        if (parameter.Type.ContainingNamespace is { } parameterNamespace)
                        {
                            yield return parameterNamespace.ToDisplayString();
                        }
                    }
                    break;

                case IPropertySymbol property:
                    if (property.Type.ContainingNamespace is { } propertyNamespace)
                    {
                        yield return propertyNamespace.ToDisplayString();
                    }
                    break;

                case IFieldSymbol field:
                    if (field.Type.ContainingNamespace is { } fieldNamespace)
                    {
                        yield return fieldNamespace.ToDisplayString();
                    }
                    break;
            }
        }
    }
}