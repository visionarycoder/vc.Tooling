using System;

namespace vc.Generators.Design;


public sealed class VcLayeringManifestGenerator : VcGeneratorBase
{
    protected override string TargetAttributeName => "VcLayeringManifest";
    protected override string Category => "Design";
    protected override string Feature => "LayeringManifest";
}

public sealed class VcModuleBoundaryMapGenerator : VcGeneratorBase
{
    protected override string TargetAttributeName => "VcModuleBoundaryMap";
    protected override string Category => "Design";
    protected override string Feature => "ModuleBoundaryMap";
}

public sealed class VcVolatilityMapGenerator : VcGeneratorBase
{
    protected override string TargetAttributeName => "VcVolatilityMap";
    protected override string Category => "Design";
    protected override string Feature => "VolatilityMap";
}

public sealed class VcDesignMetadataGenerator : VcGeneratorBase
{
    protected override string TargetAttributeName => "VcDesignMetadata";
    protected override string Category => "Design";
    protected override string Feature => "DesignMetadata";
}

public sealed class VcAdrGenerator : VcGeneratorBase
{
    protected override string TargetAttributeName => "VcAdr";
    protected override string Category => "Design";
    protected override string Feature => "Adr";
}

