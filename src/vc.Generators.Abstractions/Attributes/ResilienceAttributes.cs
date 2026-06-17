namespace Vc.Generators.Abstractions.Resilience;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcRetryPolicyAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcCircuitBreakerAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcTimeoutPolicyAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcFallbackPolicyAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcBulkheadIsolationAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public sealed class VcRetryAttribute : Attribute
{
    public int Count { get; }
    public VcRetryAttribute(int count = 3) => Count = count;
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class VcTimeoutAttribute : Attribute
{
    public int Milliseconds { get; }
    public VcTimeoutAttribute(int milliseconds) => Milliseconds = milliseconds;
}

[AttributeUsage(AttributeTargets.Method)]
public sealed class VcFallbackAttribute : Attribute
{
    public string Method { get; }
    public VcFallbackAttribute(string method) => Method = method;
}
