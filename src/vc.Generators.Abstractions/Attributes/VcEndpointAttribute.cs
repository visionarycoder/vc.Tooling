namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Declares endpoint metadata for a handler method.
/// </summary>
/// <param name="route">The route template for the endpoint.</param>
/// <param name="method">The HTTP method for the endpoint.</param>
[AttributeUsage(validOn: AttributeTargets.Method)]
public sealed class VcEndpointAttribute(string route, string method) : Attribute
{
    /// <summary>
    /// Gets the route template.
    /// </summary>
    public string Route { get; } = route;

    /// <summary>
    /// Gets the HTTP method.
    /// </summary>
    public string Method { get; } = method;
}
