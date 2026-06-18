namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as a saga.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcSagaAttribute : Attribute {}
