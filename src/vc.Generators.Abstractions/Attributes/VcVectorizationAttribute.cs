namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for vectorization-oriented optimizations.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcVectorizationAttribute : System.Attribute { }
