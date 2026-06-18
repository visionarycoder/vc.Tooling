namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as a fake builder for test data construction.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class)]
public sealed class VcFakeBuilderAttribute : System.Attribute { }
