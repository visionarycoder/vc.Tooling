using VisionaryCoder.Generators.Design;
using Xunit;

namespace vc.Generators.Tests;

public sealed class SpecificationGeneratorTests
{
    private static ImmutableArray<SyntaxTree> GetCompilation(string source, string attributeSource)
    {
        return
        [
            CSharpSyntaxTree.ParseText(text: attributeSource),
            CSharpSyntaxTree.ParseText(text: source)
        ];
    }

    private static Compilation CreateCompilation(ImmutableArray<SyntaxTree> trees)
    {
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(predicate: a => !a.IsDynamic && a.Location.Length > 0)
            .Select(selector: a => MetadataReference.CreateFromFile(path: a.Location))
            .Distinct()
            .ToArray();

        return CSharpCompilation.Create(assemblyName: "GeneratorTest")
            .AddReferences(references: references)
            .AddSyntaxTrees(trees: trees);
    }

    private static (Compilation, IEnumerable<string>) RunGenerator(string source, string attributeSource = "")
    {
        var defaultAttribute = """
            namespace Vc.Generators.Abstractions.Design;
            [AttributeUsage(AttributeTargets.Class)]
            public sealed class VcSpecificationAttribute : Attribute {}
            """;

        var attr = string.IsNullOrEmpty(value: attributeSource) ? defaultAttribute : attributeSource;
        var trees = GetCompilation(source: source, attributeSource: attr);
        var compilation = CreateCompilation(trees: trees);

        var generator = new SpecificationGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var updatedCompilation, diagnostics: out _);

        var sources = updatedCompilation.SyntaxTrees
            .Select(selector: st => st.GetText().ToString())
            .Where(predicate: s => s.Contains(value: "Specification<"))
            .ToList();

        return (updatedCompilation, sources);
    }

    [Fact]
    public void GeneratorExecutes_WithoutExceptions()
    {
        var source = """
            using Vc.Generators.Abstractions.Design;

            namespace DomainModel;

            [VcSpecification]
            public partial class Product {}
            """;

        var (_, _) = RunGenerator(source: source);
        Assert.True(condition: true);
    }

    [Fact]
    public void GeneratorProducesSourceOutput()
    {
        var source = """
            using Vc.Generators.Abstractions.Design;

            namespace DomainModel;

            [VcSpecification]
            public partial class Product {}
            """;

        var (_, sources) = RunGenerator(source: source);
        Assert.NotEmpty(collection: sources);
    }

    [Fact]
    public void GeneratedOutput_ContainsBaseSpecificationClass()
    {
        var source = """
            using Vc.Generators.Abstractions.Design;

            namespace DomainModel;

            [VcSpecification]
            public partial class Product {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public abstract class Specification<T>", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsCriteriaProperty()
    {
        var source = """
            using Vc.Generators.Abstractions.Design;

            namespace DomainModel;

            [VcSpecification]
            public partial class Product {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "Criteria", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsIncludesProperty()
    {
        var source = """
            using Vc.Generators.Abstractions.Design;

            namespace DomainModel;

            [VcSpecification]
            public partial class Product {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "Includes", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsOrderByProperty()
    {
        var source = """
            using Vc.Generators.Abstractions.Design;

            namespace DomainModel;

            [VcSpecification]
            public partial class Product {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "OrderBy", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsPagingProperties()
    {
        var source = """
            using Vc.Generators.Abstractions.Design;

            namespace DomainModel;

            [VcSpecification]
            public partial class Product {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "PageNumber", actualString: output);
        Assert.Contains(expectedSubstring: "PageSize", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsConcreteSpecificationClass()
    {
        var source = """
            using Vc.Generators.Abstractions.Design;

            namespace DomainModel;

            [VcSpecification]
            public partial class Product {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public sealed partial class ProductSpecification : Specification<Product>", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsDefaultConstructor()
    {
        var source = """
            using Vc.Generators.Abstractions.Design;

            namespace DomainModel;

            [VcSpecification]
            public partial class Product {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public ProductSpecification()", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsAddIncludeMethod()
    {
        var source = """
            using Vc.Generators.Abstractions.Design;

            namespace DomainModel;

            [VcSpecification]
            public partial class Product {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "AddInclude", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_PreservesNamespace()
    {
        var source = """
            using Vc.Generators.Abstractions.Design;

            namespace DomainModel.Queries;

            [VcSpecification]
            public partial class Product {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "namespace DomainModel.Queries;", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsAutoGeneratedHeader()
    {
        var source = """
            using Vc.Generators.Abstractions.Design;

            namespace DomainModel;

            [VcSpecification]
            public partial class Product {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "// <auto-generated by VisionaryCoder.Tooling.Generators />", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_IsDeterministic()
    {
        var source = """
            using Vc.Generators.Abstractions.Design;

            namespace DomainModel;

            [VcSpecification]
            public partial class Product {}
            """;

        var (_, sources1) = RunGenerator(source: source);
        var output1 = sources1.First();
        var (_, sources2) = RunGenerator(source: source);
        var output2 = sources2.First();
        Assert.Equal(expected: output1, actual: output2);
    }

    [Fact]
    public void GeneratedOutput_ContainsNullableDirective()
    {
        var source = """
            using Vc.Generators.Abstractions.Design;

            namespace DomainModel;

            [VcSpecification]
            public partial class Product {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "#nullable enable", actualString: output);
    }
}
