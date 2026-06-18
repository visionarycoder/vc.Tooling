namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for proxy registration generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public sealed class VcProxyRegistrationAttribute : Attribute { }
