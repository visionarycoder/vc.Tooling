namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for relay-command generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcRelayCommandAttribute : Attribute { }
