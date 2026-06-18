namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for object pooling behavior.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcPoolingAttribute : Attribute { }
