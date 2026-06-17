using System.Collections.Immutable;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace VisionaryCoder.Tooling.Generators.Tests;

public sealed class OptionsPatternGeneratorTests
{
    private static (Compilation, IEnumerable<string>) RunGenerator(string source)
    {
        const string attr = "namespace Vc.Generators.Abstractions.Dx; [System.AttributeUsage(System.AttributeTargets.Class)] public sealed class VcOptionsAttribute : System.Attribute {}";
        var trees = ImmutableArray.Create(CSharpSyntaxTree.ParseText(attr), CSharpSyntaxTree.ParseText(source));
        var refs = AppDomain.CurrentDomain.GetAssemblies().Where(a => !a.IsDynamic && a.Location.Length > 0).Select(a => MetadataReference.CreateFromFile(a.Location)).Distinct().ToArray();
        var comp = CSharpCompilation.Create("Test", trees, refs, new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));
        var driver = CSharpGeneratorDriver.Create(new OptionsPatternGenerator());
        driver.RunGeneratorsAndUpdateCompilation(comp, out var outComp, out _);
        return (outComp, outComp.SyntaxTrees.Skip(2).Select(st => st.GetText().ToString()));
    }

    [Fact]
    public void GeneratesExtensions_ForDecoratedClass()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Dx; namespace Config; [VcOptions] public class DatabaseOptions {}");
        Assert.NotEmpty(sources);
        Assert.Contains(sources, s => s.Contains("DatabaseOptionsExtensions"));
    }

    [Fact]
    public void GeneratedExtensions_ContainsDiRegistration()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Dx; namespace Config; [VcOptions] public class DatabaseOptions {}");
        var src = sources.First(s => s.Contains("DatabaseOptionsExtensions"));
        Assert.Contains("AddDatabaseOptions", src);
        Assert.Contains("IServiceCollection", src);
    }

    [Fact]
    public void GeneratedExtensions_ContainsValidator()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Dx; namespace Config; [VcOptions] public class DatabaseOptions {}");
        var src = sources.First(s => s.Contains("DatabaseOptionsExtensions"));
        Assert.Contains("DatabaseOptionsValidator", src);
        Assert.Contains("IValidateOptions", src);
    }

    [Fact]
    public void GeneratedOutput_StripsSuffixForSectionName()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Dx; namespace Config; [VcOptions] public class DatabaseOptions {}");
        var src = sources.First(s => s.Contains("DatabaseOptionsExtensions"));
        Assert.Contains("\"Database\"", src);
    }

    [Fact]
    public void GeneratedOutput_ContainsAutoGenHeader()
    {
        var (_, sources) = RunGenerator("using Vc.Generators.Abstractions.Dx; namespace Config; [VcOptions] public class DatabaseOptions {}");
        Assert.Contains(sources, s => s.Contains("auto-generated"));
    }

    [Fact]
    public void NonDecoratedClass_ProducesNoOutput()
    {
        var (_, sources) = RunGenerator("namespace Config; public class DatabaseOptions {}");
        Assert.Empty(sources);
    }
}
