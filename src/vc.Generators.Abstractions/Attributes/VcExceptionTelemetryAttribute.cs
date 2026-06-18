namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for exception telemetry instrumentation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcExceptionTelemetryAttribute : Attribute { }
