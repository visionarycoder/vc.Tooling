namespace Vc.Generators.Abstractions.Api;

[AttributeUsage(AttributeTargets.Interface)]
public sealed class VcHttpClientAttribute : Attribute {}

[AttributeUsage(AttributeTargets.Method)]
public sealed class VcEndpointAttribute : Attribute
{
    public string Route { get; }
    public string Method { get; }
    public VcEndpointAttribute(string route, string method)
    {
        Route = route;
        Method = method;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcMinimalApiAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcOpenApiAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcMessagePublisherAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcMessageSubscriberAttribute : Attribute { }
