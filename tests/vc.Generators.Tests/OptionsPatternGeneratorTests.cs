using VisionaryCoder.Generators.Design;
using Xunit;

namespace vc.Generators.Tests;

public sealed class OptionsPatternGeneratorTests
{
    private static (Compilation, IEnumerable<string>) RunGenerator(string source)
    {
        const string attr = "namespace Vc.Generators.Abstractions.Dx; [System.AttributeUsage(System.AttributeTargets.Class)] public sealed class VcOptionsAttribute : System.Attribute {}";
        var trees = ImmutableArray.Create(item1: CSharpSyntaxTree.ParseText(text: attr), item2: CSharpSyntaxTree.ParseText(text: source));
        var refs = AppDomain.CurrentDomain.GetAssemblies().Where(predicate: a => !a.IsDynamic && a.Location.Length > 0).Select(selector: a => MetadataReference.CreateFromFile(path: a.Location)).Distinct().ToArray();
        var comp = CSharpCompilation.Create(assemblyName: "Test", syntaxTrees: trees, references: refs, options: new CSharpCompilationOptions(outputKind: OutputKind.DynamicallyLinkedLibrary));
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: new OptionsPatternGenerator());
        driver.RunGeneratorsAndUpdateCompilation(compilation: comp, outputCompilation: out var outComp, diagnostics: out _);
        return (outComp, outComp.SyntaxTrees.Skip(count: 2).Select(selector: st => st.GetText().ToString()));
    }

    [Fact]
    public void GeneratesExtensions_ForDecoratedClass()
    {
        var (_, sources) = RunGenerator(source: "using Vc.Generators.Abstractions.Dx; namespace Config; [VcOptions] public class DatabaseOptions {}");
        Assert.NotEmpty(collection: sources);
        Assert.Contains(collection: sources, filter: s => s.Contains(value: "DatabaseOptionsExtensions"));
    }

    [Fact]
    public void GeneratedExtensions_ContainsDiRegistration()
    {
        var (_, sources) = RunGenerator(source: "using Vc.Generators.Abstractions.Dx; namespace Config; [VcOptions] public class DatabaseOptions {}");
        var src = sources.First(predicate: s => s.Contains(value: "DatabaseOptionsExtensions"));
        Assert.Contains(expectedSubstring: "AddDatabaseOptions", actualString: src);
        Assert.Contains(expectedSubstring: "IServiceCollection", actualString: src);
    }

    [Fact]
    public void GeneratedExtensions_ContainsValidator()
    {
        var (_, sources) = RunGenerator(source: "using Vc.Generators.Abstractions.Dx; namespace Config; [VcOptions] public class DatabaseOptions {}");
        var src = sources.First(predicate: s => s.Contains(value: "DatabaseOptionsExtensions"));
        Assert.Contains(expectedSubstring: "DatabaseOptionsValidator", actualString: src);
        Assert.Contains(expectedSubstring: "IValidateOptions", actualString: src);
    }

    [Fact]
    public void GeneratedOutput_StripsSuffixForSectionName()
    {
        var (_, sources) = RunGenerator(source: "using Vc.Generators.Abstractions.Dx; namespace Config; [VcOptions] public class DatabaseOptions {}");
        var src = sources.First(predicate: s => s.Contains(value: "DatabaseOptionsExtensions"));
        Assert.Contains(expectedSubstring: "\"Database\"", actualString: src);
    }

    [Fact]
    public void GeneratedOutput_ContainsAutoGenHeader()
    {
        var (_, sources) = RunGenerator(source: "using Vc.Generators.Abstractions.Dx; namespace Config; [VcOptions] public class DatabaseOptions {}");
        Assert.Contains(collection: sources, filter: s => s.Contains(value: "auto-generated"));
    }

    [Fact]
    public void NonDecoratedClass_ProducesNoOutput()
    {
        var (_, sources) = RunGenerator(source: "namespace Config; public class DatabaseOptions {}");
        Assert.Empty(collection: sources);
    }
}
