namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as defining authentication flow behavior.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcAuthenticationFlowAttribute : System.Attribute { }
