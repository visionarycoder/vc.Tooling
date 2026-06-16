namespace VisionaryCoder.Architecture.Vbd;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class VbdBoundaryAttribute : Attribute
{
    public VbdBoundaryAttribute(BoundaryType boundaryType)
    {
        BoundaryType = boundaryType;
    }

    public BoundaryType BoundaryType { get; }
}
