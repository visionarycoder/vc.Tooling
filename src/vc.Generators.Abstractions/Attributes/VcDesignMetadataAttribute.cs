namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type with design metadata for generation.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcDesignMetadataAttribute : System.Attribute { }
