using System.Collections.Immutable;

namespace VisionaryCoder.Generators.Common.Models;

public sealed class AttributeInfo(string Name, ImmutableDictionary<string, object?> Arguments)
{
    public string Name { get; } = Name;
    public ImmutableDictionary<string, object?> Arguments { get; } = Arguments;
}