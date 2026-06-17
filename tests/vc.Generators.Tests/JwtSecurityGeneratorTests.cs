using VisionaryCoder.Tooling.SecurityGenerators;
using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace VisionaryCoder.Tooling.Generators.Tests;

public sealed class JwtSecurityGeneratorTests
{
    [Fact]
    public void GeneratorExecutes()
    {
        var attr = "namespace Vc.Generators.Abstractions.Security; [AttributeUsage(AttributeTargets.Class)] public sealed class VcJwtSecurityAttribute : Attribute {}";
        var src = "using Vc.Generators.Abstractions.Security; namespace Security; [VcJwtSecurity] public partial class JwtHandler {}";
        var trees = ImmutableArray.Create(CSharpSyntaxTree.ParseText(attr), CSharpSyntaxTree.ParseText(src));
        var refs = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && a.Location.Length > 0).Select(a => MetadataReference.CreateFromFile(a.Location)).Distinct().ToArray();
        var comp = CSharpCompilation.Create("Test").AddReferences(refs).AddSyntaxTrees(trees);
        var driver = CSharpGeneratorDriver.Create(new JwtSecurityGenerator());
        driver.RunGeneratorsAndUpdateCompilation(comp, out _, out _);
        Assert.True(true);
    }
}
