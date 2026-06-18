namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Enables tracing metadata for a method.
/// </summary>
/// <param name="spanName">Optional span name override.</param>
[AttributeUsage(validOn: AttributeTargets.Method)]
public sealed class VcTraceAttribute(string? spanName = null) : Attribute
{
    /// <summary>
    /// Gets the optional span name override.
    /// </summary>
    public string? SpanName { get; } = spanName;
}
