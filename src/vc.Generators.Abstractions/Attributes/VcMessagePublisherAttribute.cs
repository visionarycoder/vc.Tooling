namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as a message publisher.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcMessagePublisherAttribute : Attribute { }
