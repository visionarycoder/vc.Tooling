namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as an event projection.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcEventProjectionAttribute : Attribute {}
