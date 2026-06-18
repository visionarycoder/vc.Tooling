namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as defining authorization policy behavior.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcAuthorizationPolicyAttribute : System.Attribute { }
