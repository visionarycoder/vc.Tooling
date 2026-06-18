namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for JWT validation behavior.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcJwtValidationAttribute : System.Attribute { }
