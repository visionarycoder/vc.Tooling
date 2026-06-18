namespace VisionaryCoder.Architecture;

/// <summary>
/// Represents an attribute used to define a boundary within the system architecture.
/// This attribute can be applied to assemblies, classes, interfaces, or structs
/// to categorize them into specific architectural boundaries.
/// </summary>
/// <remarks>
/// The boundary type is specified using the <see cref="BoundaryType"/> enumeration,
/// which includes values such as <c>Domain</c>, <c>Integration</c>, <c>Runtime</c>, and others.
/// </remarks>
[AttributeUsage(validOn: AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class BoundaryAttribute(BoundaryType boundaryType) : Attribute
{
    /// <summary>
    /// Gets the type of the boundary.
    /// </summary>
    public BoundaryType BoundaryType { get; } = boundaryType;
}
