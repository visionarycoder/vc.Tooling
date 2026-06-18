namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for equality implementation generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcEqualityAttribute : Attribute { }
