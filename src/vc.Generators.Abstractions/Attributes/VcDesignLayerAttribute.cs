namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Declares the design layer for a type.
/// </summary>
/// <param name="name">The design layer name.</param>
[AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public sealed class VcDesignLayerAttribute(string name) : Attribute
{
    /// <summary>
    /// Gets the design layer name.
    /// </summary>
    public string Name { get; } = name;
}
