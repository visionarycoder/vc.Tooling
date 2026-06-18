namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks an interface as an HTTP client contract.
/// </summary>
[AttributeUsage(AttributeTargets.Interface)]
public sealed class VcHttpClientAttribute : Attribute {}
