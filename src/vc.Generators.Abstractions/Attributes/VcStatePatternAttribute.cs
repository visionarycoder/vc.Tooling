namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for state-pattern generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcStatePatternAttribute : Attribute { }
