namespace Vc.Generators.Abstractions.Attributes;

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcTelemetryAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcLoggingContextAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcMetricsAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcTracingAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcExceptionTelemetryAttribute : System.Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public sealed partial class VcTelemetryAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Method)]
public sealed partial class VcTraceAttribute : Attribute
{
    public string? SpanName { get; }
    public VcTraceAttribute(string? spanName = null) => SpanName = spanName;
}

[AttributeUsage(AttributeTargets.Method)]
public sealed partial class VcMetricsAttribute : Attribute
{
    public string? Name { get; }
    public VcMetricsAttribute(string? name = null) => Name = name;
}

