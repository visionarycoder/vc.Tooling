namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for strong ID generation.
/// </summary>
[AttributeUsage(AttributeTargets.Struct | AttributeTargets.Class)]
public sealed class VcStrongIdAttribute : Attribute {}
