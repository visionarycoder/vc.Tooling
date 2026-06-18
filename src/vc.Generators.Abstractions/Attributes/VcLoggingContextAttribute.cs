namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as defining logging context metadata.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcLoggingContextAttribute : Attribute { }
