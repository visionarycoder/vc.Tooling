namespace VisionaryCoder.Tooling.Metadata;

public sealed class TypeMetadata
{
    public TypeMetadata(string typeName, string namespaceName, string assemblyName)
    {
        TypeName = typeName ?? throw new ArgumentNullException(nameof(typeName));
        NamespaceName = namespaceName ?? throw new ArgumentNullException(nameof(namespaceName));
        AssemblyName = assemblyName ?? throw new ArgumentNullException(nameof(assemblyName));
    }

    public string TypeName { get; }
    public string NamespaceName { get; }
    public string AssemblyName { get; }
    public string FullyQualifiedName => $"{NamespaceName}.{TypeName}";
    public IReadOnlyList<string> Attributes { get; init; } = [];
    public IReadOnlyDictionary<string, string> Properties { get; init; } = new Dictionary<string, string>();
}
