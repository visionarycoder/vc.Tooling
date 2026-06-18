namespace vc.Generators.Abstractions.Attributes;

/// <summary>
/// Specifies execution timeout for a method.
/// </summary>
/// <param name="milliseconds">Timeout value in milliseconds.</param>
[AttributeUsage(AttributeTargets.Method)]
public sealed class VcTimeoutAttribute(int milliseconds) : Attribute
{
    /// <summary>
    /// Gets the timeout value in milliseconds.
    /// </summary>
    public int Milliseconds { get; } = milliseconds;
}
