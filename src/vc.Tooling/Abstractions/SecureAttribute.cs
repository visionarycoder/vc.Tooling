namespace VisionaryCoder.Tooling.Abstractions;

[AttributeUsage(validOn: AttributeTargets.Class | AttributeTargets.Method)]
public sealed class SecureAttribute(string permission) : Attribute
{
    public string Permission { get; } = permission;
}