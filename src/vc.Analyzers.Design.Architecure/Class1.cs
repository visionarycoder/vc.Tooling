using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
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

    private sealed class DependencyGraph
    {
        private readonly Dictionary<string, List<(string To, INamedTypeSymbol Type)>> edges = new();

        public void AddEdge(string from, string to, INamedTypeSymbol type)
        {
            if (!edges.TryGetValue(from, out var list))
            {
                list = new List<(string, INamedTypeSymbol)>();
                edges[from] = list;
            }

            list.Add((to, type));
        }

        public IEnumerable<(string From, string To, INamedTypeSymbol Type)> FindCycles()
        {
            var visited = new HashSet<string>();
            var stack = new HashSet<string>();

            foreach (var node in edges.Keys)
            {
                foreach (var cycle in Dfs(node, visited, stack))
                {
                    yield return cycle;
                }
            }
        }

        private IEnumerable<(string From, string To, INamedTypeSymbol Type)> Dfs(
            string node,
            HashSet<string> visited,
            HashSet<string> stack)
        {
            if (stack.Contains(node))
            {
                yield break;
            }

            if (!visited.Add(node))
            {
                yield break;
            }

            stack.Add(node);

            if (edges.TryGetValue(node, out var outgoing))
            {
                foreach (var (to, type) in outgoing)
                {
                    if (stack.Contains(to))
                    {
                        yield return (node, to, type);
                    }
                    else
                    {
                        foreach (var cycle in Dfs(to, visited, stack))
                        {
                            yield return cycle;
                        }
                    }
                }
            }

            stack.Remove(node);
        }
    }
}

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class NamespaceBoundaryAnalyzer : DiagnosticAnalyzer
{
    public const string NamespaceBoundaryViolationId = "VCARCH003";

    private static readonly DiagnosticDescriptor NamespaceBoundaryViolationRule = new(
        NamespaceBoundaryViolationId,
        "Namespace boundary violation",
        "Namespace '{0}' must not reference namespace '{1}'.",
        "Architecture",
        DiagnosticSeverity.Warning,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(NamespaceBoundaryViolationRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterSymbolAction(AnalyzeType, SymbolKind.NamedType);
    }

    private static void AnalyzeType(SymbolAnalysisContext context)
    {
        if (context.Symbol is not INamedTypeSymbol typeSymbol)
        {
            return;
        }

        var fromNamespace = typeSymbol.ContainingNamespace?.ToDisplayString();
        if (string.IsNullOrEmpty(fromNamespace))
        {
            return;
        }

        foreach (var referencedNamespace in GetReferencedNamespaces(typeSymbol))
        {
            if (IsAllowedReference(fromNamespace, referencedNamespace))
            {
                continue;
            }

            context.ReportDiagnostic(
                Diagnostic.Create(
                    NamespaceBoundaryViolationRule,
                    typeSymbol.Locations.FirstOrDefault(),
                    fromNamespace,
                    referencedNamespace));
        }
    }

    private static ImmutableHashSet<string> GetReferencedNamespaces(INamedTypeSymbol typeSymbol)
    {
        var builder = ImmutableHashSet.CreateBuilder<string>();

        foreach (var member in typeSymbol.GetMembers())
        {
            switch (member)
            {
                case IMethodSymbol method:
                    AddNamespace(builder, method.ReturnType);
                    foreach (var p in method.Parameters)
                    {
                        AddNamespace(builder, p.Type);
                    }
                    break;

                case IPropertySymbol property:
                    AddNamespace(builder, property.Type);
                    break;

                case IFieldSymbol field:
                    AddNamespace(builder, field.Type);
                    break;
            }
        }

        return builder.ToImmutable();
    }

    private static void AddNamespace(ImmutableHashSet<string>.Builder builder, ITypeSymbol? type)
    {
        if (type?.ContainingNamespace is { } ns)
        {
            builder.Add(ns.ToDisplayString());
        }
    }

    private static bool IsAllowedReference(string from, string to)
    {
        // Allow self-references
        if (from == to)
        {
            return true;
        }

        // Allow references within the same root namespace
        var fromRoot = GetRoot(from);
        var toRoot = GetRoot(to);

        if (fromRoot == toRoot)
        {
            return true;
        }

        // Disallow cross-domain references:
        // Domain → Infrastructure
        // Domain → Api
        // Application → Api
        // Api → Infrastructure
        // etc.
        return false;
    }

    private static string GetRoot(string ns)
    {
        var index = ns.IndexOf('.');
        return index < 0 ? ns : ns.Substring(0, index);
    }
}

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public sealed class ProjectReferenceAnalyzer : DiagnosticAnalyzer
{
    public const string ProjectReferenceViolationId = "VCARCH004";

    private static readonly DiagnosticDescriptor ProjectReferenceViolationRule = new(
        ProjectReferenceViolationId,
        "Project reference violation",
        "Project '{0}' must not reference project '{1}'.",
        "Architecture",
        DiagnosticSeverity.Error,
        isEnabledByDefault: true);

    public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics =>
        ImmutableArray.Create(ProjectReferenceViolationRule);

    public override void Initialize(AnalysisContext context)
    {
        context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
        context.EnableConcurrentExecution();

        context.RegisterCompilationStartAction(Start);
    }

    private static void Start(CompilationStartAnalysisContext context)
    {
        var compilation = context.Compilation;
        var currentAssembly = compilation.Assembly;

        var currentLayer = GetLayer(currentAssembly.Name);

        context.RegisterSymbolAction(symbolContext =>
        {
            if (symbolContext.Symbol is not INamedTypeSymbol typeSymbol)
            {
                return;
            }

            foreach (var referencedAssembly in typeSymbol.ContainingAssembly.Modules
                .SelectMany(m => m.ReferencedAssemblySymbols))
            {
                var referencedLayer = GetLayer(referencedAssembly.Name);

                if (referencedLayer == Layer.Unknown)
                {
                    continue;
                }

                if (!IsAllowedReference(currentLayer, referencedLayer))
                {
                    symbolContext.ReportDiagnostic(
                        Diagnostic.Create(
                            ProjectReferenceViolationRule,
                            typeSymbol.Locations.FirstOrDefault(),
                            currentAssembly.Name,
                            referencedAssembly.Name));
                }
            }
        }, SymbolKind.NamedType);
    }

    private static Layer GetLayer(string assemblyName)
    {
        if (assemblyName.EndsWith(".Api"))
        {
            return Layer.Api;
        }

        if (assemblyName.EndsWith(".Application"))
        {
            return Layer.Application;
        }

        if (assemblyName.EndsWith(".Domain"))
        {
            return Layer.Domain;
        }

        if (assemblyName.EndsWith(".Infrastructure"))
        {
            return Layer.Infrastructure;
        }

        return Layer.Unknown;
    }

    private static bool IsAllowedReference(Layer from, Layer to)
    {
        if (from == to)
        {
            return true;
        }

        return (from, to) switch
        {
            (Layer.Api, Layer.Application) => true,
            (Layer.Api, Layer.Domain) => true,

            (Layer.Application, Layer.Domain) => true,

            (Layer.Infrastructure, Layer.Domain) => true,

            _ => false
        };
    }

    private enum Layer
    {
        Unknown = 0,
        Api,
        Application,
        Domain,
        Infrastructure
    }
}
