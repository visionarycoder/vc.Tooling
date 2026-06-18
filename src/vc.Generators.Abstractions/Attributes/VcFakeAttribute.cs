namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as a fake implementation for testing scenarios.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcFakeAttribute : Attribute {}
