namespace Vc.Generators.Abstractions.Proxy;

[AttributeUsage(AttributeTargets.Interface)]
public sealed partial class VcProxyAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Method)]
public sealed partial class VcBehaviorAttribute : Attribute
{
    public string Name { get; }
    public VcBehaviorAttribute(string name) => Name = name;
}

[System.AttributeUsage(System.AttributeTargets.Interface | System.AttributeTargets.Class)]
public sealed class VcProxyAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class)]
public sealed class VcProxyBehaviorListAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class)]
public sealed class VcProxyRegistrationAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcProxyContextAttribute : System.Attribute { }
