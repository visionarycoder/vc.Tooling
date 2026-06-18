namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a method as allocation-sensitive.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class VcNoAllocAttribute : Attribute {}
