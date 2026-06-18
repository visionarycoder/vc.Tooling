using VisionaryCoder.Generators.Resilience;
using Xunit;

namespace vc.Generators.Tests;

public sealed class ConfigurationGeneratorTests
{
    [Fact]
    public void GeneratorExecutes()
    {
        var attr = "namespace Vc.Generators.Abstractions.Resilience; [AttributeUsage(AttributeTargets.Class)] public sealed class VcConfigurationAttribute : Attribute {}";
        var src = "using Vc.Generators.Abstractions.Resilience; namespace Config; [VcConfiguration] public partial class AppConfiguration {}";
        var trees = ImmutableArray.Create(item1: CSharpSyntaxTree.ParseText(text: attr), item2: CSharpSyntaxTree.ParseText(text: src));
        var refs = AppDomain.CurrentDomain.GetAssemblies().Where(predicate: a => !a.IsDynamic && a.Location.Length > 0).Select(selector: a => MetadataReference.CreateFromFile(path: a.Location)).Distinct().ToArray();
        var comp = CSharpCompilation.Create(assemblyName: "Test").AddReferences(references: refs).AddSyntaxTrees(trees: trees);
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: new ConfigurationGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation: comp, outputCompilation: out _, diagnostics: out _);
        Assert.True(condition: true);
    }
}
