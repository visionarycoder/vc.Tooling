namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for timeout policy generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcTimeoutPolicyAttribute : Attribute { }
