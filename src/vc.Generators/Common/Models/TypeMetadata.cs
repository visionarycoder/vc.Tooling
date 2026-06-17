using System.Collections.Immutable;

namespace Vc.Generators.Common.Models;

public sealed class TypeMetadata(
    string Name,
    string Namespace,
    ImmutableArray<AttributeInfo> Attributes)
{
    public string Name { get; } = Name;
    public string Namespace { get; } = Namespace;
    public ImmutableArray<AttributeInfo> Attributes { get; } = Attributes;
}

public sealed class AttributeInfo(string Name, ImmutableDictionary<string, object?> Arguments)
{
    public string Name { get; } = Name;
    public ImmutableDictionary<string, object?> Arguments { get; } = Arguments;
}
