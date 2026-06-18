namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type or method as requiring JWT authentication.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class VcJwtRequiredAttribute : Attribute {}
