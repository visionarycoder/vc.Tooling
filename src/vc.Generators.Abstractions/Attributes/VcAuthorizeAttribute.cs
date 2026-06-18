namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Requires authorization for a method or type.
/// </summary>
/// <param name="policy">Optional policy name.</param>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class VcAuthorizeAttribute(string? policy = null) : Attribute
{
    /// <summary>
    /// Gets the optional policy name.
    /// </summary>
    public string? Policy { get; } = policy;
}
