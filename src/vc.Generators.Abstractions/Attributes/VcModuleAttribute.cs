namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a class as a logical module.
/// </summary>
/// <param name="name">The module name.</param>
[AttributeUsage(validOn: AttributeTargets.Class, AllowMultiple = false)]
public sealed class VcModuleAttribute(string name) : Attribute
{
    /// <summary>
    /// Gets the module name.
    /// </summary>
    public string Name { get; } = name;
}
