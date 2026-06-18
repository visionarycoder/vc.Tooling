namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for builder-pattern generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcBuilderPatternAttribute : Attribute { }
