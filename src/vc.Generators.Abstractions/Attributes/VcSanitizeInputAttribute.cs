namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a method for input sanitization behavior.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class VcSanitizeInputAttribute : Attribute {}
