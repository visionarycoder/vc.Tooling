namespace VisionaryCoder.Architecture;

[AttributeUsage(AttributeTargets.Assembly | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class BoundaryAttribute : Attribute
{
    public BoundaryAttribute(BoundaryType boundaryType)
    {
        BoundaryType = boundaryType;
    }

    public BoundaryType BoundaryType { get; }
}
