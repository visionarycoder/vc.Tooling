namespace VisionaryCoder.Architecture.Vbd;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class VbdComponentAttribute : Attribute
{
    public VbdComponentAttribute(ComponentRole role)
    {
        Role = role;
    }

    public ComponentRole Role { get; }
}
