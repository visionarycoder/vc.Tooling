using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Xunit;

namespace VisionaryCoder.Tooling.Generators.Tests;

public sealed class ResultGeneratorTests
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
            public sealed class VcResultAttribute : Attribute {}
            """;

        var attr = string.IsNullOrEmpty(attributeSource) ? defaultAttribute : attributeSource;
        var trees = GetCompilation(source, attr);
        var compilation = CreateCompilation(trees);

        var generator = new ResultGenerator();
        var driver = CSharpGeneratorDriver.Create(generator);
        driver.RunGeneratorsAndUpdateCompilation(compilation, out var updatedCompilation, out _);

        var sources = updatedCompilation.SyntaxTrees
            .Select(st => st.GetText().ToString())
            .Where(s => s.Contains("class Result"))
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

        var (_, _) = RunGenerator(source);
        Assert.True(true);
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

        var (_, sources) = RunGenerator(source);
        Assert.NotEmpty(sources);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public abstract partial class Result", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public abstract bool IsSuccess { get; }", output);
        Assert.Contains("public bool IsFailure => !IsSuccess;", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public abstract TResult Match<TResult>(", output);
        Assert.Contains("Func<object?, TResult> onSuccess,", output);
        Assert.Contains("Func<string, TResult> onFailure", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public abstract Result Bind(Func<object?, Result> binder);", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public sealed partial class Success : Result", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public sealed partial class Failure : Result", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public object? Value { get; }", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        // Success class should have IsSuccess => true
        var successIndex = output.IndexOf("public sealed partial class Success");
        var failureIndex = output.IndexOf("public sealed partial class Failure");
        var successSection = output.Substring(successIndex, failureIndex - successIndex);
        Assert.Contains("public override bool IsSuccess => true;", successSection);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public string Error { get; }", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        // Failure class should have IsSuccess => false
        var failureIndex = output.IndexOf("public sealed partial class Failure");
        var failureSection = output.Substring(failureIndex);
        Assert.Contains("public override bool IsSuccess => false;", failureSection);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public sealed partial class OperationResult", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public static Result Success(object? value)", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("public static Result Failure(string error)", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("namespace DomainModel.Queries;", output);
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

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("// <auto-generated by VisionaryCoder.Tooling.Generators />", output);
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

            namespace DomainModel;

            [VcResult]
            public partial class Operation {}
            """;

        var (_, sources) = RunGenerator(source);
        var output = sources.First();
        Assert.Contains("#nullable enable", output);
    }
}
