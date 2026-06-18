namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks an interface as an event publisher contract.
/// </summary>
[AttributeUsage(AttributeTargets.Interface)]
public sealed class VcEventPublisherAttribute : Attribute {}
