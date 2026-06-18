namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a method for deduplication behavior.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class VcDeduplicateAttribute : Attribute {}
