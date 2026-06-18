namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as a value object.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcValueObjectAttribute : Attribute {}
