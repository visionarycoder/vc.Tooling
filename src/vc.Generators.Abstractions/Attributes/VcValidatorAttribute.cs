namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as a validator.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcValidatorAttribute : Attribute { }
