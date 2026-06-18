using System.Collections.Immutable;

namespace VisionaryCoder.Generators.Common.Models;

public sealed class TypeMetadata(
    string Name,
    string Namespace,
    ImmutableArray<AttributeInfo> Attributes)
{
    public string Name { get; } = Name;
    public string Namespace { get; } = Namespace;
    public ImmutableArray<AttributeInfo> Attributes { get; } = Attributes;
}