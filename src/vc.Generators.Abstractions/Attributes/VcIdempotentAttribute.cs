namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a method as idempotent.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class VcIdempotentAttribute : Attribute {}
