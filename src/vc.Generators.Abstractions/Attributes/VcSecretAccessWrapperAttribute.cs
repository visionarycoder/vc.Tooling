namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for secret-access wrapper generation.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcSecretAccessWrapperAttribute : System.Attribute { }
