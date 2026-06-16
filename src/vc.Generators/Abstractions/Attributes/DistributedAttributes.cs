namespace Vc.Generators.Abstractions.Distributed;

[AttributeUsage(AttributeTargets.Method)]
public sealed partial class VcIdempotentAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed partial class VcOutboxAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Method)]
public sealed partial class VcDeduplicateAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed partial class VcSagaAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Method)]
public sealed partial class VcDistributedLockAttribute : Attribute {}

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcIdempotencyKeyAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcOutboxPatternAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcMessageDeduplicationAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcSagaOrchestrationAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcDistributedLockAttribute : System.Attribute { }
