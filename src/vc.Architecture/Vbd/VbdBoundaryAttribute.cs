namespace VisionaryCoder.Architecture.Vbd;

/// <summary>
/// Marks a type with its VBD boundary classification.
/// </summary>
/// <param name="boundaryType">The boundary type applied to the target component.</param>
[AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class VbdBoundaryAttribute(BoundaryType boundaryType) : Attribute
{
    /// <summary>
    /// Gets the declared boundary type for the annotated component.
    /// </summary>
    public BoundaryType BoundaryType { get; } = boundaryType;
}
