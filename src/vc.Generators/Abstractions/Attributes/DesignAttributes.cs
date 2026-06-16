namespace Vc.Generators.Abstractions.Attributes;

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcLayeringManifestAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcModuleBoundaryMapAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcVolatilityMapAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcDesignMetadataAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcAdrAttribute : System.Attribute { }


[AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false)]
public sealed partial class VcDesignLayerAttribute : Attribute
{
    public string Name { get; }
    public VcDesignLayerAttribute(string name) => Name = name;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed partial class VcModuleAttribute : Attribute
{
    public string Name { get; }
    public VcModuleAttribute(string name) => Name = name;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed partial class VcVolatilityAttribute : Attribute
{
    public string Level { get; }
    public VcVolatilityAttribute(string level) => Level = level;
}

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcAdrAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcLayeringManifestAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcModuleBoundaryMapAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcVolatilityMapAttribute : System.Attribute { }

[System.AttributeUsage(System.AttributeTargets.Class | System.AttributeTargets.Struct)]
public sealed class VcDesignMetadataAttribute : System.Attribute { }
