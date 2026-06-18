namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Specifies a fallback method for a method.
/// </summary>
/// <param name="method">The fallback method name.</param>
[AttributeUsage(validOn: AttributeTargets.Method)]
public sealed class VcFallbackAttribute(string method) : Attribute
{
    /// <summary>
    /// Gets the fallback method name.
    /// </summary>
    public string Method { get; } = method;
}
