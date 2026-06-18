namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for fallback policy generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcFallbackPolicyAttribute : Attribute { }
