namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as an aggregate root.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcAggregateRootAttribute : Attribute {}
