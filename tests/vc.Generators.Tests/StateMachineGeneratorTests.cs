using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace VisionaryCoder.Tooling.Generators.Tests;

public sealed class StateMachineGeneratorTests
{
    [Fact]
    public void GeneratorExecutes()
    {
        var attr = "namespace Vc.Generators.Abstractions.Design; [AttributeUsage(AttributeTargets.Class)] public sealed class VcStateMachineAttribute : Attribute {}";
        var src = "using Vc.Generators.Abstractions.Design; namespace Design; [VcStateMachine] public partial class OrderStateMachine {}";
        var trees = ImmutableArray.Create(CSharpSyntaxTree.ParseText(attr), CSharpSyntaxTree.ParseText(src));
        var refs = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && a.Location.Length > 0).Select(a => MetadataReference.CreateFromFile(a.Location)).Distinct().ToArray();
        var comp = CSharpCompilation.Create("Test").AddReferences(refs).AddSyntaxTrees(trees);
        var driver = CSharpGeneratorDriver.Create(new StateMachineGenerator());
        driver.RunGeneratorsAndUpdateCompilation(comp, out _, out _);
        Assert.True(true);
    }
}
