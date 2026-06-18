namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for outbox pattern policy generation.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcOutboxPatternAttribute : System.Attribute { }
