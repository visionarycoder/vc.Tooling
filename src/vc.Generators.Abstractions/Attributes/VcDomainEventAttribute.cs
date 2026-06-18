namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as a domain event.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcDomainEventAttribute : Attribute {}
