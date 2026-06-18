namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for message deduplication behavior.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcMessageDeduplicationAttribute : System.Attribute { }
