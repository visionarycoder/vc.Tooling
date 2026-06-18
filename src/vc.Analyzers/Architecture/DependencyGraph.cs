namespace VisionaryCoder.Analyzers.Architecture;

internal sealed class DependencyGraph
{
    private readonly Dictionary<string, List<(string To, INamedTypeSymbol Type)>> edges = new();

    public void AddEdge(string from, string to, INamedTypeSymbol type)
    {
        if (!edges.TryGetValue(key: from, value: out var list))
        {
            list = [];
            edges[key: from] = list;
        }

        list.Add(item: (to, type));
    }

    public IEnumerable<(string From, string To, INamedTypeSymbol Type)> FindCycles()
    {
        var visited = new HashSet<string>();
        var stack = new HashSet<string>();

        foreach (var node in edges.Keys)
        {
            foreach (var cycle in Dfs(node: node, visited: visited, stack: stack))
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
        if (stack.Contains(item: node))
        {
            yield break;
        }

        if (!visited.Add(item: node))
        {
            yield break;
        }

        stack.Add(item: node);

        if (edges.TryGetValue(key: node, value: out var outgoing))
        {
            foreach (var (to, type) in outgoing)
            {
                if (stack.Contains(item: to))
                {
                    yield return (node, to, type);
                }
                else
                {
                    foreach (var cycle in Dfs(node: to, visited: visited, stack: stack))
                    {
                        yield return cycle;
                    }
                }
            }
        }

        stack.Remove(item: node);
    }
}
