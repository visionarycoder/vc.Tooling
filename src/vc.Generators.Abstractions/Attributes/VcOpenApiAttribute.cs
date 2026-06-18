namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for OpenAPI metadata generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcOpenApiAttribute : Attribute { }
