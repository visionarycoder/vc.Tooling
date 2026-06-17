using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace VisionaryCoder.Tooling.Generators.Tests;

public sealed class UnionGeneratorTests
{
    [Fact]
    public void GeneratorExecutes()
    {
        var attr = "namespace Vc.Generators.Abstractions.Dx; [AttributeUsage(AttributeTargets.Class)] public sealed class VcUnionAttribute : Attribute {}";
        var src = "using Vc.Generators.Abstractions.Dx; namespace Types; [VcUnion] public partial class Result {}";
        var trees = ImmutableArray.Create(CSharpSyntaxTree.ParseText(attr), CSharpSyntaxTree.ParseText(src));
        var refs = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && a.Location.Length > 0).Select(a => MetadataReference.CreateFromFile(a.Location)).Distinct().ToArray();
        var comp = CSharpCompilation.Create("Test").AddReferences(refs).AddSyntaxTrees(trees);
        var driver = CSharpGeneratorDriver.Create(new UnionGenerator());
        driver.RunGeneratorsAndUpdateCompilation(comp, out _, out _);
        Assert.True(true);
    }
}
