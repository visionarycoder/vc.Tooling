namespace Vc.Generators.Abstractions.Design;

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
public sealed class VcDesignLayerAttribute : Attribute
{
    public string Name { get; }
    public VcDesignLayerAttribute(string name) => Name = name;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class VcModuleAttribute : Attribute
{
    public string Name { get; }
    public VcModuleAttribute(string name) => Name = name;
}

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class VcVolatilityAttribute : Attribute
{
    public string Level { get; }
    public VcVolatilityAttribute(string level) => Level = level;
}

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcValidatorAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcSpecificationAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcBuilderAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Interface)]
public sealed class VcFactoryAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcPolymorphicAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcStatePatternAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Class)]
public sealed class VcRelayCommandAttribute : Attribute { }
