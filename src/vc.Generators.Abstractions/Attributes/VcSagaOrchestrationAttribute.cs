namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type for saga orchestration behavior.
/// </summary>
[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcSagaOrchestrationAttribute : System.Attribute { }
