namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for builder generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcBuilderAttribute : Attribute { }
