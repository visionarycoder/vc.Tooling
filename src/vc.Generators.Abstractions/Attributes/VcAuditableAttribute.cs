namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as auditable.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcAuditableAttribute : Attribute {}
