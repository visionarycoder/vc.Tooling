namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Declares a named behavior for a method.
/// </summary>
/// <param name="name">The behavior name.</param>
[AttributeUsage(AttributeTargets.Method)]
public sealed class VcBehaviorAttribute(string name) : Attribute
{
    /// <summary>
    /// Gets the behavior name.
    /// </summary>
    public string Name { get; } = name;
}
