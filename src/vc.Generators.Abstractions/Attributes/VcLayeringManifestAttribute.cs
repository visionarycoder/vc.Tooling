namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as contributing layering manifest metadata.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcLayeringManifestAttribute : System.Attribute { }
