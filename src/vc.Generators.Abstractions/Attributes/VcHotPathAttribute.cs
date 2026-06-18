namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a method as a performance-critical hot path.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class VcHotPathAttribute : Attribute {}
