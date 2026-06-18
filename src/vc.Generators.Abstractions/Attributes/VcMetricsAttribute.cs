namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for metrics instrumentation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcMetricsAttribute : Attribute { }
