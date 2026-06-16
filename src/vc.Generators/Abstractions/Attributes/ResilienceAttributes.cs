namespace Vc.Generators.Abstractions.Attributes;

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcRetryPolicyAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcCircuitBreakerAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcTimeoutPolicyAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcFallbackPolicyAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcBulkheadIsolationAttribute : System.Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public sealed partial class VcRetryAttribute : Attribute
{
    public int Count { get; }
    public VcRetryAttribute(int count = 3) => Count = count;
}

[AttributeUsage(AttributeTargets.Method)]
public sealed partial class VcCircuitBreakerAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Method)]
public sealed partial class VcTimeoutAttribute : Attribute
{
    public int Milliseconds { get; }
    public VcTimeoutAttribute(int milliseconds) => Milliseconds = milliseconds;
}

[AttributeUsage(AttributeTargets.Method)]
public sealed partial class VcFallbackAttribute : Attribute
{
    public string Method { get; }
    public VcFallbackAttribute(string method) => Method = method;
}
