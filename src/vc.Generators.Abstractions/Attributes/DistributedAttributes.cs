namespace Vc.Generators.Abstractions.Distributed;

[AttributeUsage(AttributeTargets.Method)]
public sealed class VcIdempotentAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcOutboxAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Method)]
public sealed class VcDeduplicateAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcSagaAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Method)]
public sealed class VcDistributedLockAttribute : Attribute {}

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcIdempotencyKeyAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcOutboxPatternAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcMessageDeduplicationAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcSagaOrchestrationAttribute : System.Attribute { }
