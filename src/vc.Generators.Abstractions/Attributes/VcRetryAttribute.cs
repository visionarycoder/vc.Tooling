namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Configures retry behavior for a method.
/// </summary>
/// <param name="count">The maximum retry attempts.</param>
[AttributeUsage(validOn: AttributeTargets.Method)]
public sealed class VcRetryAttribute(int count = 3) : Attribute
{
    /// <summary>
    /// Gets the maximum retry attempts.
    /// </summary>
    public int Count { get; } = count;
}
