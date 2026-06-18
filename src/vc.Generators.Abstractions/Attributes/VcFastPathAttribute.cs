namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as optimized for fast-path execution.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcFastPathAttribute : System.Attribute { }
