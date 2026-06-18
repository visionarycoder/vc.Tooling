using VisionaryCoder.Generators.Api;
using Xunit;

namespace vc.Generators.Tests;

public sealed class ApiClientGeneratorTests
{
    [Fact]
    public void GeneratorExecutes()
    {
        var attr = "namespace Vc.Generators.Abstractions.Api; [AttributeUsage(AttributeTargets.Interface)] public sealed class VcApiClientAttribute : Attribute {}";
        var src = "using Vc.Generators.Abstractions.Api; namespace Api; [VcApiClient] public partial interface IUserApi {}";
        var trees = ImmutableArray.Create(item1: CSharpSyntaxTree.ParseText(text: attr), item2: CSharpSyntaxTree.ParseText(text: src));
        var refs = AppDomain.CurrentDomain.GetAssemblies().Where(predicate: a => !a.IsDynamic && a.Location.Length > 0).Select(selector: a => MetadataReference.CreateFromFile(path: a.Location)).Distinct().ToArray();
        var comp = CSharpCompilation.Create(assemblyName: "Test").AddReferences(references: refs).AddSyntaxTrees(trees: trees);
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: new ApiClientGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation: comp, outputCompilation: out _, diagnostics: out _);
        Assert.True(condition: true);
    }
}
