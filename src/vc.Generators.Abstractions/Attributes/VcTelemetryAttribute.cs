namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for telemetry instrumentation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcTelemetryAttribute : Attribute { }
