namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks an interface as a factory contract.
/// </summary>
[AttributeUsage(AttributeTargets.Interface)]
public sealed class VcFactoryAttribute : Attribute { }
