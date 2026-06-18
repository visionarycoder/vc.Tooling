namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as using an outbox pattern.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcOutboxAttribute : Attribute {}
