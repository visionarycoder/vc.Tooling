namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for property-change notification generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcNotifyPropertyChangedAttribute : Attribute {}
