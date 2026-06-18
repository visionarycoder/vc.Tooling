namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for bulkhead isolation policies.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcBulkheadIsolationAttribute : Attribute { }
