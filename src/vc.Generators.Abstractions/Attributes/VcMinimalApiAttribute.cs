namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as part of a minimal API surface.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcMinimalApiAttribute : Attribute { }
