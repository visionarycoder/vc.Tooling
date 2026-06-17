using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace VisionaryCoder.Tooling.Generators.Tests;

public sealed class SpecificationGeneratorTests
{
    private static ImmutableArray<SyntaxTree> GetCompilation(string source, string attributeSource)
    {
        return ImmutableArray.Create(
            CSharpSyntaxTree.ParseText(attributeSource),
            CSharpSyntaxTree.ParseText(source)
        );
    }

    private static Compilation CreateCompilation(ImmutableArray<SyntaxTree> trees)
    {
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => !a.IsDynamic && a.Location.Length > 0)
            .Select(a => MetadataReference.CreateFromFile(a.Location))
            .Distinct()
            .ToArray();

        return CSharpCompilation.Create("GeneratorTest")
            .AddReferences(references)
            .AddSyntaxTrees(trees);
    }

    private static (Compilation, IEnumerable<string>) RunGenerator(string source, string attributeSource = "")
    {
        var defaultAttribute = """
            namespace Vc.Generators.Abstractions.Design;
            [AttributeUsage(AttributeTargets.Class)]
            public sealed class VcSpecificationAttribute : Attribute {}
            """;

        var attr = string.IsNullOrEmpty(attributeSource) ? defaultAttribute : attributeSource;
        var trees = GetCompilation(source, attr);
        var compilation = CreateCompilation(trees);

        var generator = new SpecificationGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out _);

        var sources = updatedCompilation.SyntaxTrees
            .Select(st => st.GetText().ToString())
            .Where(s => s.Contains("Specification<"))
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

        var (_, _) = RunGenerator(source);
        Assert.True(true);
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

        var (_, sources) = RunGenerator(source);
        Assert.NotEmpty(sources);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public abstract class Specification<T>", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("Criteria", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("Includes", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("OrderBy", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("PageNumber", output);
        Assert.Contains("PageSize", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public sealed partial class ProductSpecification : Specification<Product>", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public ProductSpecification()", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("AddInclude", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("namespace DomainModel.Queries;", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("// <auto-generated by VisionaryCoder.Tooling.Generators />", output);
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

        var (_, sources1) = RunGenerator(source);
        var output1 = sources1.First();
        var (_, sources2) = RunGenerator(source);
        var output2 = sources2.First();
        Assert.Equal(output1, output2);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("#nullable enable", output);
    }
}
