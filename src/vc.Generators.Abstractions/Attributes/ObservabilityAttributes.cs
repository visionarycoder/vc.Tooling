namespace Vc.Generators.Abstractions.Attributes;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcTelemetryAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcLoggingContextAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcMetricsAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcTracingAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcExceptionTelemetryAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public sealed class VcTraceAttribute : Attribute
{
    public string? SpanName { get; }
    public VcTraceAttribute(string? spanName = null) => SpanName = spanName;
}
