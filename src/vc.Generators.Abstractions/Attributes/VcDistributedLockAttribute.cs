namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a method to use distributed locking semantics.
/// </summary>
[AttributeUsage(AttributeTargets.Method)]
public sealed class VcDistributedLockAttribute : Attribute {}
