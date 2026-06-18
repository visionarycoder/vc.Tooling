namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for span-based optimization behavior.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcSpanOptimizationAttribute : Attribute { }
