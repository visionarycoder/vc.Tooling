namespace VisionaryCoder.Tooling.Abstractions;

[AttributeUsage(validOn: AttributeTargets.Class)]
public sealed class PermissionGroupAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}