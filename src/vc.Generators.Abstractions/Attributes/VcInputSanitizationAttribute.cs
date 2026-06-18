namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for input sanitization behavior.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcInputSanitizationAttribute : System.Attribute { }
