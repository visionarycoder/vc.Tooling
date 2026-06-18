namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type as a message subscriber.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcMessageSubscriberAttribute : Attribute { }
