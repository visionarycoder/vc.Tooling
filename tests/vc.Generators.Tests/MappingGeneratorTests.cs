using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace VisionaryCoder.Tooling.Generators.Tests;

public sealed class MappingGeneratorTests
{
    [Fact]
    public void GeneratorExecutes()
    {
        var attr = "namespace Vc.Generators.Abstractions.Dx; [AttributeUsage(AttributeTargets.Class)] public sealed class VcMappingAttribute : Attribute {}";
        var src = "using Vc.Generators.Abstractions.Dx; namespace Maps; [VcMapping] public partial class UserMapping {}";
        var trees = ImmutableArray.Create(CSharpSyntaxTree.ParseText(attr), CSharpSyntaxTree.ParseText(src));
        var refs = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && a.Location.Length > 0).Select(a => MetadataReference.CreateFromFile(a.Location)).Distinct().ToArray();
        var comp = CSharpCompilation.Create("Test").AddReferences(refs).AddSyntaxTrees(trees);
        var driver = CSharpGeneratorDriver.Create(new MappingGenerator());
        driver.RunGeneratorsAndUpdateCompilation(comp, out _, out _);
        Assert.True(true);
    }
}
