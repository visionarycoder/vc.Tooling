namespace Vc.Generators.Abstractions.Api;

[AttributeUsage(AttributeTargets.Interface)]
public sealed partial class VcHttpClientAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Method)]
public sealed partial class VcEndpointAttribute : Attribute
{
    public string Route { get; }
    public string Method { get; }
    public VcEndpointAttribute(string route, string method)
    {
        Route = route;
        Method = method;
    }
}

[System.AttributeUsage(System.AttributeTargets.Interface | System.AttributeTargets.Class)]
public sealed class VcHttpClientAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class)]
public sealed class VcMinimalApiAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class)]
public sealed class VcOpenApiAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class)]
public sealed class VcMessagePublisherAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class)]
public sealed class VcMessageSubscriberAttribute : System.Attribute { }
