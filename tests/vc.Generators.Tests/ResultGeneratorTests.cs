using VisionaryCoder.Generators.Domain;
using Xunit;

namespace vc.Generators.Tests;

public sealed class ResultGeneratorTests
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
            namespace Vc.Generators.Abstractions.Domain;
            [AttributeUsage(AttributeTargets.Class)]
            public sealed class VcResultAttribute : Attribute {}
            """;

        var attr = string.IsNullOrEmpty(value: attributeSource) ? defaultAttribute : attributeSource;
        var trees = GetCompilation(source: source, attributeSource: attr);
        var compilation = CreateCompilation(trees: trees);

        var generator = new ResultGenerator();
        var driver = CSharpGeneratorDriver.Create(incrementalGenerators: generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation: compilation, outputCompilation: out var updatedCompilation, diagnostics: out _);

        var sources = updatedCompilation.SyntaxTrees
            .Select(selector: st => st.GetText().ToString())
            .Where(predicate: s => s.Contains(value: "class Result"))
            .ToList();

        return (updatedCompilation, sources);
    }

    [Fact]
    public void GeneratorExecutes_WithoutExceptions()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, _) = RunGenerator(source: source);
        Assert.True(condition: true);
    }

    [Fact]
    public void GeneratorProducesSourceOutput()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source: source);
        Assert.NotEmpty(collection: sources);
    }

    [Fact]
    public void GeneratedOutput_ContainsResultBaseClass()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public abstract partial class Result", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsResultProperties()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public abstract bool IsSuccess { get; }", actualString: output);
        Assert.Contains(expectedSubstring: "public bool IsFailure => !IsSuccess;", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsResultMatchMethod()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public abstract TResult Match<TResult>(", actualString: output);
        Assert.Contains(expectedSubstring: "Func<object?, TResult> onSuccess,", actualString: output);
        Assert.Contains(expectedSubstring: "Func<string, TResult> onFailure", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsResultBindMethod()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public abstract Result Bind(Func<object?, Result> binder);", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsSuccessClass()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public sealed partial class Success : Result", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsFailureClass()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public sealed partial class Failure : Result", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_SuccessContainsValueProperty()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public object? Value { get; }", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_SuccessContainsIsSuccessTrue()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        // Success class should have IsSuccess => true
        var successIndex = output.IndexOf(value: "public sealed partial class Success");
        var failureIndex = output.IndexOf(value: "public sealed partial class Failure");
        var successSection = output.Substring(startIndex: successIndex, length: failureIndex - successIndex);
        Assert.Contains(expectedSubstring: "public override bool IsSuccess => true;", actualString: successSection);
    }

    [Fact]
    public void GeneratedOutput_FailureContainsErrorProperty()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public string Error { get; }", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_FailureContainsIsSuccessFalse()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        // Failure class should have IsSuccess => false
        var failureIndex = output.IndexOf(value: "public sealed partial class Failure");
        var failureSection = output.Substring(startIndex: failureIndex);
        Assert.Contains(expectedSubstring: "public override bool IsSuccess => false;", actualString: failureSection);
    }

    [Fact]
    public void GeneratedOutput_ContainsResultTypeFactory()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public sealed partial class OperationResult", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_FactoryContainsSuccessMethod()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public static Result Success(object? value)", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_FactoryContainsFailureMethod()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "public static Result Failure(string error)", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_PreservesNamespace()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel.Queries;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "namespace DomainModel.Queries;", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_ContainsAutoGeneratedHeader()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "// <auto-generated by VisionaryCoder.Tooling.Generators />", actualString: output);
    }

    [Fact]
    public void GeneratedOutput_IsDeterministic()
    {
        var source = """
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
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
            using Vc.Generators.Abstractions.Domain;

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source: source);
        var output = sources.First();
        Assert.Contains(expectedSubstring: "#nullable enable", actualString: output);
    }
}
