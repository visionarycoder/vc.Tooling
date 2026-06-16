using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace Vc.Analyzers.Architecture;

internal sealed class DependencyGraph
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
