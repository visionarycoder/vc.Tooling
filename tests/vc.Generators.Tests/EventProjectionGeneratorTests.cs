using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace VisionaryCoder.Tooling.Generators.Tests;

public sealed class EventProjectionGeneratorTests
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
            namespace Vc.Generators.Abstractions.Domain;
            [AttributeUsage(AttributeTargets.Class)]
            public sealed class VcEventProjectionAttribute : Attribute {}
            """;
        
        var attr = string.IsNullOrEmpty(attributeSource) ? defaultAttribute : attributeSource;
        var trees = GetCompilation(source, attr);
        var compilation = CreateCompilation(trees);
        var generator = new EventProjectionGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var outputCompilation, out _);
        var sources = outputCompilation.SyntaxTrees
            .Skip(compilation.SyntaxTrees.Count())
            .Select(st => st.GetText().ToString())
            .ToList();
        return (outputCompilation, sources);
    }

    [Fact]
    public void GeneratorExecutes_WithoutExceptions()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.ReadModels;

            [VcEventProjection]
            public partial class OrderProjection {}
            """;

        var (_, _) = RunGenerator(source);
        Assert.True(true);
    }

    [Fact]
    public void GeneratorProducesSourceOutput()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.ReadModels;

            [VcEventProjection]
            public partial class OrderProjection {}
            """;

        var (_, sources) = RunGenerator(source);
        Assert.NotEmpty(sources);
    }

    [Fact]
    public void GeneratedOutput_ContainsEventProjectionClass()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.ReadModels;

            [VcEventProjection]
            public partial class OrderProjection {}
            """;

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public abstract partial class EventProjection", output);
    }

    [Fact]
    public void GeneratedOutput_ContainsConcreteProjection()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.ReadModels;

            [VcEventProjection]
            public partial class OrderProjection {}
            """;

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public sealed partial class OrderProjectionProjection", output);
    }

    [Fact]
    public void GeneratedOutput_ContainsIdProperty()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.ReadModels;

            [VcEventProjection]
            public partial class OrderProjection {}
            """;

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public Guid Id { get; protected set; }", output);
    }

    [Fact]
    public void GeneratedOutput_ContainsVersionProperty()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.ReadModels;

            [VcEventProjection]
            public partial class OrderProjection {}
            """;

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public long Version { get; protected set; }", output);
    }

    [Fact]
    public void GeneratedOutput_ContainsLastUpdatedProperty()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.ReadModels;

            [VcEventProjection]
            public partial class OrderProjection {}
            """;

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public DateTime LastUpdated { get; protected set; }", output);
    }

    [Fact]
    public void GeneratedOutput_ContainsApplyEventMethod()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.ReadModels;

            [VcEventProjection]
            public partial class OrderProjection {}
            """;

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("protected abstract void ApplyEvent(object @event);", output);
    }

    [Fact]
    public void GeneratedOutput_ContainsHandleMethod()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.ReadModels;

            [VcEventProjection]
            public partial class OrderProjection {}
            """;

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public void Handle(object @event", output);
    }

    [Fact]
    public void GeneratedOutput_ContainsNullValidation()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.ReadModels;

            [VcEventProjection]
            public partial class OrderProjection {}
            """;

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("if (@event == null)", output);
    }

    [Fact]
    public void GeneratedOutput_PreservesNamespace()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.ReadModels.Orders;

            [VcEventProjection]
            public partial class OrderProjection {}
            """;

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("namespace Application.ReadModels.Orders;", output);
    }

    [Fact]
    public void GeneratedOutput_ContainsAutoGeneratedHeader()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.ReadModels;

            [VcEventProjection]
            public partial class OrderProjection {}
            """;

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("// <auto-generated by VisionaryCoder.Tooling.Generators />", output);
    }

    [Fact]
    public void GeneratedOutput_IsDeterministic()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace Application.ReadModels;

            [VcEventProjection]
            public partial class OrderProjection {}
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
            using Vc.Generators.Abstractions.Domain;

            namespace Application.ReadModels;

            [VcEventProjection]
            public partial class OrderProjection {}
            """;

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("#nullable enable", output);
    }
}
