namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for allocation reduction optimizations.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcAllocationReductionAttribute : System.Attribute { }
