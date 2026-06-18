namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for tracing instrumentation.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcTracingAttribute : Attribute { }
