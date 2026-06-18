namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as carrying idempotency key semantics.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcIdempotencyKeyAttribute : System.Attribute { }
