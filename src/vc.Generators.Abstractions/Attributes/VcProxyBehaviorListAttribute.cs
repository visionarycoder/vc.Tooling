namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as declaring proxy behavior list metadata.
/// </summary>
[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public sealed class VcProxyBehaviorListAttribute : Attribute { }
