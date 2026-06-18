namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type or method for pooling behavior.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class VcPoolAttribute : Attribute {}
