namespace Vc.Generators.Abstractions.Performance;

[AttributeUsage(AttributeTargets.Method)]
public sealed class VcHotPathAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed class VcPoolAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Method)]
public sealed class VcNoAllocAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcSpanOptimizationAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcPoolingAttribute : Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcFastPathAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcVectorizationAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcAllocationReductionAttribute : System.Attribute { }
