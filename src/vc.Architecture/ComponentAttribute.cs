namespace VisionaryCoder.Architecture;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Struct, AllowMultiple = false, Inherited = false)]
public sealed class ComponentAttribute : Attribute
{
    public ComponentAttribute(ComponentRole role)
    {
        Role = role;
    }

    public ComponentRole Role { get; }
}
