namespace Vc.Generators.Abstractions.Proxy;

[AttributeUsage(AttributeTargets.Interface)]
public sealed class VcProxyAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Method)]
public sealed class VcBehaviorAttribute : Attribute
{
    public string Name { get; }
    public VcBehaviorAttribute(string name) => Name = name;
}

[AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
public sealed class VcProxyBehaviorListAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcProxyRegistrationAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct)]
public sealed class VcProxyContextAttribute : Attribute { }
