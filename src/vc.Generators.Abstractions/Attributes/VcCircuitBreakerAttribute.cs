namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for circuit-breaker policy generation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcCircuitBreakerAttribute : Attribute { }
