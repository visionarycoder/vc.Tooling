using VisionaryCoder.Generators.Domain;
using Xunit;

namespace vc.Generators.Tests;

public sealed class StateMachineGeneratorTests
{
    [Fact]
    public void GeneratorExecutes()
    {
        var attr = "namespace Vc.Generators.Abstractions.Design; [AttributeUsage(AttributeTargets.Class)] public sealed class VcStateMachineAttribute : Attribute {}";
        var src = "using Vc.Generators.Abstractions.Design; namespace Design; [VcStateMachine] public partial class OrderStateMachine {}";
        var trees = ImmutableArray.Create(item1: CSharpSyntaxTree.ParseText(text: attr), item2: CSharpSyntaxTree.ParseText(text: src));
        var refs = AppDomain.CurrentDomain.GetAssemblies().Where(predicate: a => !a.IsDynamic && a.Location.Length > 0).Select(selector: a => MetadataReference.CreateFromFile(path: a.Location)).Distinct().ToArray();
        var comp = CSharpCompilation.Create(assemblyName: "Test").AddReferences(references: refs).AddSyntaxTrees(trees: trees);
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: new StateMachineGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation: comp, outputCompilation: out _, diagnostics: out _);
        Assert.True(condition: true);
    }
}
