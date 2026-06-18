namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as containing secret-related metadata.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcSecretAttribute : Attribute {}
