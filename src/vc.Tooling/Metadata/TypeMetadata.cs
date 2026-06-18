namespace VisionaryCoder.Tooling.Metadata;

public sealed class TypeMetadata(string typeName, string namespaceName, string assemblyName)
{
    public string TypeName { get; } = typeName ?? throw new ArgumentNullException(paramName: nameof(typeName));
    public string NamespaceName { get; } = namespaceName ?? throw new ArgumentNullException(paramName: nameof(namespaceName));
    public string AssemblyName { get; } = assemblyName ?? throw new ArgumentNullException(paramName: nameof(assemblyName));
    public string FullyQualifiedName => $"{NamespaceName}.{TypeName}";
    public IReadOnlyList<string> Attributes { get; init; } = [];
    public IReadOnlyDictionary<string, string> Properties { get; init; } = new Dictionary<string, string>();
}
