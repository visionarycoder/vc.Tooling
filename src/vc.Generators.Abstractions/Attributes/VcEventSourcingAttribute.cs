namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as event-sourced.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcEventSourcingAttribute : Attribute {}
