namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Marks a type with its volatility classification.
/// </summary>
/// <param name="level">The volatility level label.</param>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class VcVolatilityAttribute(string level) : Attribute
{
    /// <summary>
    /// Gets the volatility level of the class.
    /// </summary>
    public string Level { get; } = level;
}
