namespace Vc.Generators.Abstractions.Performance;

[AttributeUsage(AttributeTargets.Method)]
public sealed partial class VcHotPathAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public sealed partial class VcPoolAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Method)]
public sealed partial class VcNoAllocAttribute : Attribute {}

namespace Vc.Generators.Abstractions.Attributes;

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcSpanOptimizationAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcPoolingAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcFastPathAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcVectorizationAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcAllocationReductionAttribute : System.Attribute { }
