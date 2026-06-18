namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for polymorphic contract generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcPolymorphicAttribute : Attribute { }
