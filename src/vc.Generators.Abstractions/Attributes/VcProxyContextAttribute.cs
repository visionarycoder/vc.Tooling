namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as a proxy execution context.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcProxyContextAttribute : Attribute { }
