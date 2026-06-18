namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as participating in ADR-oriented generation.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcAdrAttribute : System.Attribute { }
